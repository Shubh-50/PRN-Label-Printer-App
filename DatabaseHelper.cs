using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace BarcodeBartenderApp
{
    public static class DatabaseHelper
    {
        private static string dbPath = "users.db";
        private static string connectionString =
            $"Data Source={dbPath};Version=3;BusyTimeout=5000;";

        public static void Initialize()
        {
            if (!File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);

            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();

                new SQLiteCommand("PRAGMA journal_mode=WAL;", con).ExecuteNonQuery();
                MigratePdfSettings(con);

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Users (
                    Username TEXT PRIMARY KEY,
                    Password TEXT,
                    IsFirstLogin INTEGER)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS EmailSettings(
                    Sender TEXT,
                    Password TEXT,
                    Receiver TEXT)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS ShiftSettings(
                    ShiftName TEXT,
                    StartTime TEXT,
                    EndTime TEXT)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS PdfSettings(
                    PartName TEXT,
                    FilePath TEXT)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Parts(
                    PartName TEXT PRIMARY KEY)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS SerialSettings(
                    Id INTEGER PRIMARY KEY,
                    CurrentSerial INTEGER)", con).ExecuteNonQuery();

                new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS ScanLog(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DateTime TEXT,
                    Barcode TEXT,
                    PartName TEXT,
                    Username TEXT,
                    Shift TEXT,
                    Reprinted INTEGER DEFAULT 0,
                    ReprintReason TEXT)", con).ExecuteNonQuery();

                // Default Admin
                var adminCount = (long)new SQLiteCommand(
                    "SELECT COUNT(*) FROM Users WHERE Username='admin'", con).ExecuteScalar();
                if (adminCount == 0)
                    new SQLiteCommand("INSERT INTO Users VALUES ('admin','1234',1)", con).ExecuteNonQuery();

                // Default Email Row
                var emailCount = (long)new SQLiteCommand(
                    "SELECT COUNT(*) FROM EmailSettings", con).ExecuteScalar();
                if (emailCount == 0)
                    new SQLiteCommand("INSERT INTO EmailSettings VALUES('','','')", con).ExecuteNonQuery();

                // Default Shifts
                var shiftCount = (long)new SQLiteCommand(
                    "SELECT COUNT(*) FROM ShiftSettings", con).ExecuteScalar();
                if (shiftCount == 0)
                {
                    new SQLiteCommand(@"INSERT INTO ShiftSettings VALUES
                        ('A','06:00','14:00'),
                        ('B','14:00','22:00'),
                        ('C','22:00','06:00')", con).ExecuteNonQuery();
                }

                // Default Serial
                var serialCount = (long)new SQLiteCommand(
                    "SELECT COUNT(*) FROM SerialSettings", con).ExecuteScalar();
                if (serialCount == 0)
                    new SQLiteCommand("INSERT INTO SerialSettings VALUES(1, 500)", con).ExecuteNonQuery();
            }
        }

        private static void MigratePdfSettings(SQLiteConnection con)
        {
            try
            {
                // Check if PartName column exists
                var cmd = new SQLiteCommand(
                    "SELECT PartName FROM PdfSettings LIMIT 1", con);
                cmd.ExecuteScalar();
            }
            catch
            {
                // Column doesn't exist — migrate!
                new SQLiteCommand("DROP TABLE IF EXISTS PdfSettings", con).ExecuteNonQuery();
                new SQLiteCommand(@"CREATE TABLE PdfSettings (
            PartName TEXT,
            FilePath TEXT)", con).ExecuteNonQuery();
            }
        }

        // ================= USER =================

        public static bool ValidateUser(string username, string password, out bool isFirstLogin)
        {
            isFirstLogin = false;
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    con.Open();
                    var cmd = new SQLiteCommand(
                        "SELECT Password, IsFirstLogin FROM Users WHERE Username=@u", con);
                    cmd.Parameters.AddWithValue("@u", username);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string dbPass = reader["Password"]?.ToString() ?? "";
                        isFirstLogin = Convert.ToInt32(reader["IsFirstLogin"]) == 1;
                        return dbPass == password;
                    }
                }
            }
            catch { }
            return false;
        }

        public static void UpdatePassword(string username, string newPassword)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "UPDATE Users SET Password=@p, IsFirstLogin=0 WHERE Username=@u", con);
                cmd.Parameters.AddWithValue("@p", newPassword);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool AddUser(string user, string pass)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    con.Open();
                    var cmd = new SQLiteCommand("INSERT INTO Users VALUES (@u,@p,1)", con);
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", pass);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public static void DeleteUser(string username)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand("DELETE FROM Users WHERE Username=@u", con);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<string> GetUsers()
        {
            var list = new List<string>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var reader = new SQLiteCommand("SELECT Username FROM Users", con).ExecuteReader();
                while (reader.Read())
                    list.Add(reader["Username"]?.ToString() ?? "");
            }
            return list;
        }

        // ================= PART =================

        public static List<string> GetParts()
        {
            var list = new List<string>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var reader = new SQLiteCommand("SELECT PartName FROM Parts", con).ExecuteReader();
                while (reader.Read())
                    list.Add(reader["PartName"]?.ToString() ?? "");
            }
            return list;
        }

        public static bool AddPart(string part)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    con.Open();
                    var cmd = new SQLiteCommand("INSERT INTO Parts VALUES (@p)", con);
                    cmd.Parameters.AddWithValue("@p", part);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public static void DeletePart(string part)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand("DELETE FROM Parts WHERE PartName=@p", con);
                cmd.Parameters.AddWithValue("@p", part);
                cmd.ExecuteNonQuery();
            }
        }

        // ================= EMAIL =================

        public static void SaveEmailSettings(string sender, string password, string receiver)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "UPDATE EmailSettings SET Sender=@s, Password=@p, Receiver=@r", con);
                cmd.Parameters.AddWithValue("@s", sender);
                cmd.Parameters.AddWithValue("@p", password);
                cmd.Parameters.AddWithValue("@r", receiver);
                cmd.ExecuteNonQuery();
            }
        }

        public static (string sender, string password, string receiver) GetEmailSettings()
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var reader = new SQLiteCommand(
                    "SELECT * FROM EmailSettings LIMIT 1", con).ExecuteReader();
                if (reader.Read())
                    return (
                        reader["Sender"]?.ToString() ?? "",
                        reader["Password"]?.ToString() ?? "",
                        reader["Receiver"]?.ToString() ?? "");
            }
            return ("", "", "");
        }

        // ================= SHIFT =================

        public static List<(string shift, TimeSpan start, TimeSpan end)> GetShifts()
        {
            var list = new List<(string, TimeSpan, TimeSpan)>();
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var reader = new SQLiteCommand("SELECT * FROM ShiftSettings", con).ExecuteReader();
                while (reader.Read())
                {
                    string shift = reader["ShiftName"]?.ToString() ?? "";
                    TimeSpan.TryParse(reader["StartTime"]?.ToString(), out TimeSpan s);
                    TimeSpan.TryParse(reader["EndTime"]?.ToString(), out TimeSpan e);
                    list.Add((shift, s, e));
                }
            }
            return list;
        }

        public static void UpdateShift(string shift, string start, string end)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "UPDATE ShiftSettings SET StartTime=@s, EndTime=@e WHERE ShiftName=@shift", con);
                cmd.Parameters.AddWithValue("@s", start);
                cmd.Parameters.AddWithValue("@e", end);
                cmd.Parameters.AddWithValue("@shift", shift);
                cmd.ExecuteNonQuery();
            }
        }

        // ================= PDF =================

        public static string GetPdfPath(string partName = "")
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                SQLiteCommand cmd;
                if (!string.IsNullOrEmpty(partName))
                {
                    cmd = new SQLiteCommand(
                        "SELECT FilePath FROM PdfSettings WHERE PartName=@p LIMIT 1", con);
                    cmd.Parameters.AddWithValue("@p", partName);
                }
                else
                {
                    cmd = new SQLiteCommand(
                        "SELECT FilePath FROM PdfSettings LIMIT 1", con);
                }
                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }

        public static void SavePdfPath(string partName, string filePath)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var check = new SQLiteCommand(
                    "SELECT COUNT(*) FROM PdfSettings WHERE PartName=@p", con);
                check.Parameters.AddWithValue("@p", partName);
                long count = (long)check.ExecuteScalar();

                if (count > 0)
                {
                    var update = new SQLiteCommand(
                        "UPDATE PdfSettings SET FilePath=@f WHERE PartName=@p", con);
                    update.Parameters.AddWithValue("@f", filePath);
                    update.Parameters.AddWithValue("@p", partName);
                    update.ExecuteNonQuery();
                }
                else
                {
                    var insert = new SQLiteCommand(
                        "INSERT INTO PdfSettings VALUES(@p,@f)", con);
                    insert.Parameters.AddWithValue("@p", partName);
                    insert.Parameters.AddWithValue("@f", filePath);
                    insert.ExecuteNonQuery();
                }
            }
        }

        // ================= SERIAL =================

        public static int GetSerial()
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var result = new SQLiteCommand(
                    "SELECT CurrentSerial FROM SerialSettings WHERE Id=1", con).ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 500;
            }
        }

        public static void SaveSerial(int value)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "UPDATE SerialSettings SET CurrentSerial=@v WHERE Id=1", con);
                cmd.Parameters.AddWithValue("@v", value);
                cmd.ExecuteNonQuery();
            }
        }

        // ================= SCAN LOG =================

        public static void SaveScanLog(string barcode, string part, string user, string shift)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(@"INSERT INTO ScanLog
                    (DateTime, Barcode, PartName, Username, Shift, Reprinted)
                    VALUES (@dt, @b, @p, @u, @s, 0)", con);
                cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@b", barcode);
                cmd.Parameters.AddWithValue("@p", part);
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@s", shift);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool IsDuplicate(string barcode)
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "SELECT COUNT(*) FROM ScanLog WHERE Barcode=@b AND Reprinted=0", con);
                cmd.Parameters.AddWithValue("@b", barcode);
                return (long)cmd.ExecuteScalar() > 0;
            }
        }

        public static int GetTodayCount()
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                var cmd = new SQLiteCommand(
                    "SELECT COUNT(*) FROM ScanLog WHERE DATE(DateTime)=DATE('now')", con);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static int GetTotalCount()
        {
            using (var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                return Convert.ToInt32(new SQLiteCommand(
                    "SELECT COUNT(*) FROM ScanLog", con).ExecuteScalar());
            }
        }
    }
}