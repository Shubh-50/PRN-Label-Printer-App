using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace BarcodeBartenderApp
{
    public static class DatabaseHelper
    {
        private static string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "BarcodeApp", "users.db");
        private static string connectionString =>
            $"Data Source={dbPath};Version=3;BusyTimeout=5000;";

        public static string AppVersion = "v3.0";

        public static void Initialize()
        {
            string folder = Path.GetDirectoryName(dbPath)!;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            if (!File.Exists(dbPath)) SQLiteConnection.CreateFile(dbPath);

            using var con = new SQLiteConnection(connectionString);
            con.Open();
            new SQLiteCommand("PRAGMA journal_mode=WAL;", con).ExecuteNonQuery();

            MigratePdfSettings(con);

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Users (
                Username TEXT PRIMARY KEY, Password TEXT, IsFirstLogin INTEGER)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS EmailSettings(
                Sender TEXT, Password TEXT, Receiver TEXT)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS ShiftSettings(
                ShiftName TEXT, StartTime TEXT, EndTime TEXT)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS PdfSettings(
                PartName TEXT, FilePath TEXT)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Parts(
                PartName TEXT PRIMARY KEY)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS SerialSettings(
                Id INTEGER PRIMARY KEY, CurrentSerial INTEGER)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS ScanLog(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DateTime TEXT, Barcode TEXT, PartName TEXT,
                Username TEXT, Shift TEXT,
                Reprinted INTEGER DEFAULT 0, ReprintReason TEXT,
                Inspector TEXT DEFAULT '')", con).ExecuteNonQuery();

            // Migrate: add Inspector column if missing
            try { new SQLiteCommand("ALTER TABLE ScanLog ADD COLUMN Inspector TEXT DEFAULT ''", con).ExecuteNonQuery(); }
            catch { /* already exists */ }

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS ShiftTarget(
                Shift TEXT PRIMARY KEY, Target INTEGER DEFAULT 0)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS AppConfig(
                Key TEXT PRIMARY KEY, Value TEXT)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS PartPrn(
                PartName TEXT PRIMARY KEY, PrnContent TEXT, PrnPath TEXT)", con).ExecuteNonQuery();

            // ── Dispatch tables ──────────────────────────────────────────
            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS DispatchOrders(
                OrderNo       TEXT PRIMARY KEY,
                CreatedDate   TEXT,
                DueDate       TEXT,
                CustomerName  TEXT,
                PartName      TEXT,
                QRReference   TEXT,
                QtyOrdered    INTEGER DEFAULT 0,
                QtyScanned    INTEGER DEFAULT 0,
                Status        TEXT DEFAULT 'Pending',
                LockedBy      TEXT DEFAULT '',
                CompletedAt   TEXT,
                CreatedBy     TEXT)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS DispatchScanLog(
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderNo    TEXT,
                Barcode    TEXT,
                ScanTime   TEXT,
                Operator   TEXT,
                Result     TEXT)", con).ExecuteNonQuery();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS CustomerPrn(
                CustomerName TEXT PRIMARY KEY,
                PrnContent   TEXT,
                PrnPath      TEXT)", con).ExecuteNonQuery();
            // ────────────────────────────────────────────────────────────

            var adminCheck = (long)new SQLiteCommand(
                "SELECT COUNT(*) FROM Users WHERE Username='admin'", con).ExecuteScalar();
            if (adminCheck == 0)
                new SQLiteCommand("INSERT INTO Users VALUES('admin','1234',1)", con).ExecuteNonQuery();

            var emailCheck = (long)new SQLiteCommand(
                "SELECT COUNT(*) FROM EmailSettings", con).ExecuteScalar();
            if (emailCheck == 0)
                new SQLiteCommand("INSERT INTO EmailSettings VALUES('','','')", con).ExecuteNonQuery();

            var shiftCheck = (long)new SQLiteCommand(
                "SELECT COUNT(*) FROM ShiftSettings", con).ExecuteScalar();
            if (shiftCheck == 0)
                new SQLiteCommand(@"INSERT INTO ShiftSettings VALUES
                    ('A','06:00','14:00'),('B','14:00','22:00'),('C','22:00','06:00')", con).ExecuteNonQuery();

            var serialCheck = (long)new SQLiteCommand(
                "SELECT COUNT(*) FROM SerialSettings", con).ExecuteScalar();
            if (serialCheck == 0)
                new SQLiteCommand("INSERT INTO SerialSettings VALUES(1,500)", con).ExecuteNonQuery();

            foreach (var s in new[] { "A", "B", "C" })
            {
                var tc = (long)new SQLiteCommand(
                    $"SELECT COUNT(*) FROM ShiftTarget WHERE Shift='{s}'", con).ExecuteScalar();
                if (tc == 0)
                    new SQLiteCommand($"INSERT INTO ShiftTarget VALUES('{s}',0)", con).ExecuteNonQuery();
            }

            SetConfig(con, "PrinterShareName", "TSC_TE244");
            SetConfig(con, "AppVersion", AppVersion);
            SetConfig(con, "YellowDaysBeforeDue", "1");
            SetConfig(con, "RedDaysBeforeDue", "0");
        }

        // ================= CONFIG =================

        private static void SetConfig(SQLiteConnection con, string key, string value)
        {
            var check = (long)new SQLiteCommand(
                $"SELECT COUNT(*) FROM AppConfig WHERE Key='{key}'", con).ExecuteScalar();
            if (check == 0)
            {
                var cmd = new SQLiteCommand("INSERT INTO AppConfig VALUES(@k,@v)", con);
                cmd.Parameters.AddWithValue("@k", key);
                cmd.Parameters.AddWithValue("@v", value);
                cmd.ExecuteNonQuery();
            }
        }

        public static string GetConfig(string key)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand("SELECT Value FROM AppConfig WHERE Key=@k", con);
            cmd.Parameters.AddWithValue("@k", key);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        public static void SaveConfig(string key, string value)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand("INSERT OR REPLACE INTO AppConfig VALUES(@k,@v)", con);
            cmd.Parameters.AddWithValue("@k", key);
            cmd.Parameters.AddWithValue("@v", value);
            cmd.ExecuteNonQuery();
        }

        private static void MigratePdfSettings(SQLiteConnection con)
        {
            try { new SQLiteCommand("SELECT PartName FROM PdfSettings LIMIT 1", con).ExecuteScalar(); }
            catch
            {
                new SQLiteCommand("DROP TABLE IF EXISTS PdfSettings", con).ExecuteNonQuery();
                new SQLiteCommand("CREATE TABLE PdfSettings(PartName TEXT, FilePath TEXT)", con).ExecuteNonQuery();
            }
        }

        // ================= USER =================

        public static bool ValidateUser(string username, string password, out bool isFirstLogin)
        {
            isFirstLogin = false;
            try
            {
                using var con = new SQLiteConnection(connectionString);
                con.Open();
                var cmd = new SQLiteCommand(
                    "SELECT Password, IsFirstLogin FROM Users WHERE Username=@u", con);
                cmd.Parameters.AddWithValue("@u", username);
                var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    string dbPass = r["Password"]?.ToString() ?? "";
                    isFirstLogin = Convert.ToInt32(r["IsFirstLogin"]) == 1;
                    return dbPass == password;
                }
            }
            catch { }
            return false;
        }

        public static void UpdatePassword(string username, string newPassword)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "UPDATE Users SET Password=@p, IsFirstLogin=0 WHERE Username=@u", con);
            cmd.Parameters.AddWithValue("@p", newPassword);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.ExecuteNonQuery();
        }

        public static bool AddUser(string user, string pass)
        {
            try
            {
                using var con = new SQLiteConnection(connectionString);
                con.Open();
                var cmd = new SQLiteCommand("INSERT INTO Users VALUES(@u,@p,1)", con);
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@p", pass);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        public static void DeleteUser(string username)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand("DELETE FROM Users WHERE Username=@u", con);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.ExecuteNonQuery();
        }

        public static List<string> GetUsers()
        {
            var list = new List<string>();
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var r = new SQLiteCommand("SELECT Username FROM Users", con).ExecuteReader();
            while (r.Read()) list.Add(r["Username"]?.ToString() ?? "");
            return list;
        }

        // ================= PART =================

        public static List<string> GetParts()
        {
            var list = new List<string>();
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var r = new SQLiteCommand("SELECT PartName FROM Parts", con).ExecuteReader();
            while (r.Read()) list.Add(r["PartName"]?.ToString() ?? "");
            return list;
        }

        public static bool AddPart(string part)
        {
            try
            {
                using var con = new SQLiteConnection(connectionString);
                con.Open();
                var cmd = new SQLiteCommand("INSERT INTO Parts VALUES(@p)", con);
                cmd.Parameters.AddWithValue("@p", part);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        public static void DeletePart(string part)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand("DELETE FROM Parts WHERE PartName=@p", con);
            cmd.Parameters.AddWithValue("@p", part);
            cmd.ExecuteNonQuery();
        }

        // ================= EMAIL =================

        public static void SaveEmailSettings(string sender, string password, string receiver)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "UPDATE EmailSettings SET Sender=@s, Password=@p, Receiver=@r", con);
            cmd.Parameters.AddWithValue("@s", sender);
            cmd.Parameters.AddWithValue("@p", password);
            cmd.Parameters.AddWithValue("@r", receiver);
            cmd.ExecuteNonQuery();
        }

        public static (string sender, string password, string receiver) GetEmailSettings()
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var r = new SQLiteCommand("SELECT * FROM EmailSettings LIMIT 1", con).ExecuteReader();
            if (r.Read())
                return (r["Sender"]?.ToString() ?? "",
                        r["Password"]?.ToString() ?? "",
                        r["Receiver"]?.ToString() ?? "");
            return ("", "", "");
        }

        // ================= SHIFT =================

        public static List<(string shift, TimeSpan start, TimeSpan end)> GetShifts()
        {
            var list = new List<(string, TimeSpan, TimeSpan)>();
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var r = new SQLiteCommand("SELECT * FROM ShiftSettings", con).ExecuteReader();
            while (r.Read())
            {
                string shift = r["ShiftName"]?.ToString() ?? "";
                TimeSpan.TryParse(r["StartTime"]?.ToString(), out TimeSpan s);
                TimeSpan.TryParse(r["EndTime"]?.ToString(), out TimeSpan e);
                list.Add((shift, s, e));
            }
            return list;
        }

        public static void UpdateShift(string shift, string start, string end)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "UPDATE ShiftSettings SET StartTime=@s, EndTime=@e WHERE ShiftName=@shift", con);
            cmd.Parameters.AddWithValue("@s", start);
            cmd.Parameters.AddWithValue("@e", end);
            cmd.Parameters.AddWithValue("@shift", shift);
            cmd.ExecuteNonQuery();
        }

        // ================= TARGET =================

        public static int GetShiftTarget(string shift)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand("SELECT Target FROM ShiftTarget WHERE Shift=@s", con);
            cmd.Parameters.AddWithValue("@s", shift);
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public static void SaveShiftTarget(string shift, int target)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand("INSERT OR REPLACE INTO ShiftTarget VALUES(@s,@t)", con);
            cmd.Parameters.AddWithValue("@s", shift);
            cmd.Parameters.AddWithValue("@t", target);
            cmd.ExecuteNonQuery();
        }

        // ================= PDF =================

        public static string GetPdfPath(string partName = "")
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand cmd;
            if (!string.IsNullOrEmpty(partName))
            {
                cmd = new SQLiteCommand(
                    "SELECT FilePath FROM PdfSettings WHERE PartName=@p LIMIT 1", con);
                cmd.Parameters.AddWithValue("@p", partName);
            }
            else
                cmd = new SQLiteCommand("SELECT FilePath FROM PdfSettings LIMIT 1", con);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        public static void SavePdfPath(string partName, string filePath)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var check = new SQLiteCommand(
                "SELECT COUNT(*) FROM PdfSettings WHERE PartName=@p", con);
            check.Parameters.AddWithValue("@p", partName);
            long count = (long)check.ExecuteScalar();
            if (count > 0)
            {
                var upd = new SQLiteCommand(
                    "UPDATE PdfSettings SET FilePath=@f WHERE PartName=@p", con);
                upd.Parameters.AddWithValue("@f", filePath);
                upd.Parameters.AddWithValue("@p", partName);
                upd.ExecuteNonQuery();
            }
            else
            {
                var ins = new SQLiteCommand("INSERT INTO PdfSettings VALUES(@p,@f)", con);
                ins.Parameters.AddWithValue("@p", partName);
                ins.Parameters.AddWithValue("@f", filePath);
                ins.ExecuteNonQuery();
            }
        }

        // ================= SERIAL =================

        public static int GetSerial()
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var result = new SQLiteCommand(
                "SELECT CurrentSerial FROM SerialSettings WHERE Id=1", con).ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 500;
        }

        public static void SaveSerial(int value)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "UPDATE SerialSettings SET CurrentSerial=@v WHERE Id=1", con);
            cmd.Parameters.AddWithValue("@v", value);
            cmd.ExecuteNonQuery();
        }

        // ================= SCAN LOG =================

        public static void SaveScanLog(string barcode, string part,
            string user, string shift, bool reprinted = false, string reason = "",
            string inspector = "")
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(@"INSERT INTO ScanLog
                (DateTime,Barcode,PartName,Username,Shift,Reprinted,ReprintReason,Inspector)
                VALUES(@dt,@b,@p,@u,@s,@r,@rr,@ins)", con);
            cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@b", barcode);
            cmd.Parameters.AddWithValue("@p", part);
            cmd.Parameters.AddWithValue("@u", user);
            cmd.Parameters.AddWithValue("@s", shift);
            cmd.Parameters.AddWithValue("@r", reprinted ? 1 : 0);
            cmd.Parameters.AddWithValue("@rr", reason);
            cmd.Parameters.AddWithValue("@ins", inspector);
            cmd.ExecuteNonQuery();
        }

        public static bool IsDuplicate(string barcode)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT COUNT(*) FROM ScanLog WHERE Barcode=@b AND Reprinted=0", con);
            cmd.Parameters.AddWithValue("@b", barcode);
            return (long)cmd.ExecuteScalar() > 0;
        }

        public static int GetTodayCount()
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            return Convert.ToInt32(new SQLiteCommand(
                "SELECT COUNT(*) FROM ScanLog WHERE DATE(DateTime)=DATE('now')", con).ExecuteScalar());
        }

        public static int GetShiftCount(string shift)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT COUNT(*) FROM ScanLog WHERE Shift=@s AND DATE(DateTime)=DATE('now')", con);
            cmd.Parameters.AddWithValue("@s", shift);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int GetTotalCount()
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            return Convert.ToInt32(new SQLiteCommand(
                "SELECT COUNT(*) FROM ScanLog", con).ExecuteScalar());
        }

        // ================= PRN CONFIG =================

        public static string GetPrnContent(string partName)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT PrnContent FROM PartPrn WHERE PartName=@p", con);
            cmd.Parameters.AddWithValue("@p", partName);
            string content = cmd.ExecuteScalar()?.ToString() ?? "";
            if (!string.IsNullOrEmpty(content))
                content = content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
            return content;
        }

        public static string GetPrnPath(string partName)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT PrnPath FROM PartPrn WHERE PartName=@p", con);
            cmd.Parameters.AddWithValue("@p", partName);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        public static void SavePrnConfig(string partName, string content, string path)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            if (!string.IsNullOrEmpty(content))
                content = content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
            var cmd = new SQLiteCommand(@"INSERT OR REPLACE INTO PartPrn
                (PartName,PrnContent,PrnPath) VALUES(@n,@c,@p)", con);
            cmd.Parameters.AddWithValue("@n", partName);
            cmd.Parameters.AddWithValue("@c", content);
            cmd.Parameters.AddWithValue("@p", path);
            cmd.ExecuteNonQuery();
        }

        // ================= DISPATCH ORDERS =================

        public static string GenerateOrderNo()
        {
            string today = DateTime.Today.ToString("yyyyMMdd");
            string prefix = $"DSP-{today}-";
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT OrderNo FROM DispatchOrders WHERE OrderNo LIKE @p ORDER BY OrderNo DESC LIMIT 1", con);
            cmd.Parameters.AddWithValue("@p", prefix + "%");
            string? last = cmd.ExecuteScalar()?.ToString();
            int next = 1;
            if (!string.IsNullOrEmpty(last))
            {
                string suffix = last.Substring(prefix.Length);
                if (int.TryParse(suffix, out int n)) next = n + 1;
            }
            return $"{prefix}{next:D3}";
        }

        public static void SaveDispatchOrder(string orderNo, string dueDate,
            string customer, string partName, string qrRef,
            int qty, string createdBy)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(@"INSERT OR REPLACE INTO DispatchOrders
                (OrderNo,CreatedDate,DueDate,CustomerName,PartName,QRReference,
                 QtyOrdered,QtyScanned,Status,LockedBy,CreatedBy)
                VALUES(@on,@cd,@dd,@cu,@pn,@qr,@qty,0,'Pending','',@cb)", con);
            cmd.Parameters.AddWithValue("@on", orderNo);
            cmd.Parameters.AddWithValue("@cd", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@dd", dueDate);
            cmd.Parameters.AddWithValue("@cu", customer);
            cmd.Parameters.AddWithValue("@pn", partName);
            cmd.Parameters.AddWithValue("@qr", qrRef);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@cb", createdBy);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateDispatchOrder(string orderNo, string customer,
            string partName, string qrRef, int qty, string dueDate)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(@"UPDATE DispatchOrders
                SET CustomerName=@cu, PartName=@pn, QRReference=@qr,
                    QtyOrdered=@qty, DueDate=@dd
                WHERE OrderNo=@on", con);
            cmd.Parameters.AddWithValue("@cu", customer);
            cmd.Parameters.AddWithValue("@pn", partName);
            cmd.Parameters.AddWithValue("@qr", qrRef);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@dd", dueDate);
            cmd.Parameters.AddWithValue("@on", orderNo);
            cmd.ExecuteNonQuery();
        }

        public static List<DispatchOrder> GetDispatchOrders(bool todayOnly = false)
        {
            var list = new List<DispatchOrder>();
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            string filter = todayOnly
                ? "WHERE Status != 'Done' OR DATE(CreatedDate)=DATE('now')"
                : "";
            var r = new SQLiteCommand(
                $"SELECT * FROM DispatchOrders {filter} ORDER BY DueDate ASC, CreatedDate ASC", con)
                .ExecuteReader();
            while (r.Read())
                list.Add(new DispatchOrder
                {
                    OrderNo = r["OrderNo"]?.ToString() ?? "",
                    CreatedDate = r["CreatedDate"]?.ToString() ?? "",
                    DueDate = r["DueDate"]?.ToString() ?? "",
                    CustomerName = r["CustomerName"]?.ToString() ?? "",
                    PartName = r["PartName"]?.ToString() ?? "",
                    QRReference = r["QRReference"]?.ToString() ?? "",
                    QtyOrdered = Convert.ToInt32(r["QtyOrdered"]),
                    QtyScanned = Convert.ToInt32(r["QtyScanned"]),
                    Status = r["Status"]?.ToString() ?? "Pending",
                    LockedBy = r["LockedBy"]?.ToString() ?? "",
                    CompletedAt = r["CompletedAt"]?.ToString() ?? "",
                    CreatedBy = r["CreatedBy"]?.ToString() ?? ""
                });
            return list;
        }

        public static DispatchOrder? GetDispatchOrder(string orderNo)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT * FROM DispatchOrders WHERE OrderNo=@o", con);
            cmd.Parameters.AddWithValue("@o", orderNo);
            var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new DispatchOrder
            {
                OrderNo = r["OrderNo"]?.ToString() ?? "",
                CreatedDate = r["CreatedDate"]?.ToString() ?? "",
                DueDate = r["DueDate"]?.ToString() ?? "",
                CustomerName = r["CustomerName"]?.ToString() ?? "",
                PartName = r["PartName"]?.ToString() ?? "",
                QRReference = r["QRReference"]?.ToString() ?? "",
                QtyOrdered = Convert.ToInt32(r["QtyOrdered"]),
                QtyScanned = Convert.ToInt32(r["QtyScanned"]),
                Status = r["Status"]?.ToString() ?? "Pending",
                LockedBy = r["LockedBy"]?.ToString() ?? "",
                CompletedAt = r["CompletedAt"]?.ToString() ?? "",
                CreatedBy = r["CreatedBy"]?.ToString() ?? ""
            };
        }

        public static void IncrementDispatchScan(string orderNo)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            new SQLiteCommand(
                "UPDATE DispatchOrders SET QtyScanned=QtyScanned+1, Status='InProgress' WHERE OrderNo=@o",
                con)
            { Parameters = { new SQLiteParameter("@o", orderNo) } }.ExecuteNonQuery();
        }

        public static void CompleteDispatchOrder(string orderNo)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "UPDATE DispatchOrders SET Status='Done', CompletedAt=@t, LockedBy='' WHERE OrderNo=@o", con);
            cmd.Parameters.AddWithValue("@t", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@o", orderNo);
            cmd.ExecuteNonQuery();
        }

        public static void LockDispatchOrder(string orderNo, string username)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "UPDATE DispatchOrders SET LockedBy=@u WHERE OrderNo=@o", con);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@o", orderNo);
            cmd.ExecuteNonQuery();
        }

        public static void UnlockDispatchOrder(string orderNo)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "UPDATE DispatchOrders SET LockedBy='' WHERE OrderNo=@o", con);
            cmd.Parameters.AddWithValue("@o", orderNo);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteDispatchOrder(string orderNo)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "DELETE FROM DispatchOrders WHERE OrderNo=@o", con);
            cmd.Parameters.AddWithValue("@o", orderNo);
            cmd.ExecuteNonQuery();
        }

        public static void SaveDispatchScan(string orderNo, string barcode,
            string op, string result)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(@"INSERT INTO DispatchScanLog
                (OrderNo,Barcode,ScanTime,Operator,Result)
                VALUES(@o,@b,@t,@op,@r)", con);
            cmd.Parameters.AddWithValue("@o", orderNo);
            cmd.Parameters.AddWithValue("@b", barcode);
            cmd.Parameters.AddWithValue("@t", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@op", op);
            cmd.Parameters.AddWithValue("@r", result);
            cmd.ExecuteNonQuery();
        }

        public static List<DispatchScanEntry> GetDispatchScans(string orderNo)
        {
            var list = new List<DispatchScanEntry>();
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT * FROM DispatchScanLog WHERE OrderNo=@o ORDER BY ScanTime ASC", con);
            cmd.Parameters.AddWithValue("@o", orderNo);
            var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new DispatchScanEntry
                {
                    OrderNo = r["OrderNo"]?.ToString() ?? "",
                    Barcode = r["Barcode"]?.ToString() ?? "",
                    ScanTime = r["ScanTime"]?.ToString() ?? "",
                    Operator = r["Operator"]?.ToString() ?? "",
                    Result = r["Result"]?.ToString() ?? ""
                });
            return list;
        }

        // ================= CUSTOMER PRN =================

        public static string GetCustomerPrnContent(string customerName)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT PrnContent FROM CustomerPrn WHERE CustomerName=@c", con);
            cmd.Parameters.AddWithValue("@c", customerName);
            string content = cmd.ExecuteScalar()?.ToString() ?? "";
            if (!string.IsNullOrEmpty(content))
                content = content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
            return content;
        }

        public static string GetCustomerPrnPath(string customerName)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var cmd = new SQLiteCommand(
                "SELECT PrnPath FROM CustomerPrn WHERE CustomerName=@c", con);
            cmd.Parameters.AddWithValue("@c", customerName);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        public static void SaveCustomerPrn(string customerName, string content, string path)
        {
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            if (!string.IsNullOrEmpty(content))
                content = content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
            var cmd = new SQLiteCommand(@"INSERT OR REPLACE INTO CustomerPrn
                (CustomerName,PrnContent,PrnPath) VALUES(@c,@ct,@p)", con);
            cmd.Parameters.AddWithValue("@c", customerName);
            cmd.Parameters.AddWithValue("@ct", content);
            cmd.Parameters.AddWithValue("@p", path);
            cmd.ExecuteNonQuery();
        }

        public static List<string> GetCustomers()
        {
            var list = new List<string>();
            using var con = new SQLiteConnection(connectionString);
            con.Open();
            var r = new SQLiteCommand(
                "SELECT DISTINCT CustomerName FROM DispatchOrders ORDER BY CustomerName", con)
                .ExecuteReader();
            while (r.Read()) list.Add(r["CustomerName"]?.ToString() ?? "");
            return list;
        }
    }

    // ================= MODELS =================

    public class DispatchOrder
    {
        public string OrderNo { get; set; } = "";
        public string CreatedDate { get; set; } = "";
        public string DueDate { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string PartName { get; set; } = "";
        public string QRReference { get; set; } = "";
        public int QtyOrdered { get; set; }
        public int QtyScanned { get; set; }
        public string Status { get; set; } = "Pending";
        public string LockedBy { get; set; } = "";
        public string CompletedAt { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int QtyPending => QtyOrdered - QtyScanned;
    }

    public class DispatchScanEntry
    {
        public string OrderNo { get; set; } = "";
        public string Barcode { get; set; } = "";
        public string ScanTime { get; set; } = "";
        public string Operator { get; set; } = "";
        public string Result { get; set; } = "";
    }
}