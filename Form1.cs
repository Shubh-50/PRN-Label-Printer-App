using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public partial class Form1 : Form
    {
        // ── state ────────────────────────────────────────────────────────
        private string printerShareName = "";
        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeApp");
        private int totalCount = 0;
        private int todayCount = 0;
        private int shiftCount = 0;
        private int shiftTarget = 0;
        private int serialNumber = 500;
        public string CurrentUser = "";
        private string currentShift = "";
        private string lastShift = "";
        private string mailSentForShift = "";
        private string currentInspector = "";   // set from txtInspector each scan

        // ── dispatch state ───────────────────────────────────────────────
        private DispatchOrder? activeOrder = null;
        private readonly Stopwatch scanTimer = new Stopwatch();
        private DateTime lastKeystroke = DateTime.MinValue;

        private System.Windows.Forms.Timer timerClock = new System.Windows.Forms.Timer();

        public Form1(string user)
        {
            InitializeComponent();
            CurrentUser = user;
            printerShareName = DatabaseHelper.GetConfig("PrinterShareName");
            serialNumber = DatabaseHelper.GetSerial();
            timerClock.Interval = 1000;
            timerClock.Tick += TimerClock_Tick;
            timerClock.Start();
        }

        // ── timer ────────────────────────────────────────────────────────

        private void TimerClock_Tick(object? sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            string newShift = ShiftHelper.GetCurrentShift();
            if (newShift != lastShift)
            {
                if (!string.IsNullOrEmpty(lastShift) && mailSentForShift != newShift)
                {
                    SendShiftReport();
                    mailSentForShift = newShift;
                }
                lastShift = newShift;
                currentShift = newShift;
                lblShift.Text = "Shift: " + newShift;
                shiftCount = DatabaseHelper.GetShiftCount(newShift);
                shiftTarget = DatabaseHelper.GetShiftTarget(newShift);
                UpdateProgress();
            }
        }

        public void RefreshShiftTarget()
        {
            shiftTarget = DatabaseHelper.GetShiftTarget(currentShift);
            shiftCount = DatabaseHelper.GetShiftCount(currentShift);
            UpdateProgress();
        }

        // ── load ─────────────────────────────────────────────────────────

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(baseFolder)) Directory.CreateDirectory(baseFolder);

            this.Text = $"Dispatch System — {DatabaseHelper.AppVersion}";
            lblUser.Text = "User: " + CurrentUser;
            lblStatus.Text = "READY";
            lblStatus.ForeColor = Color.FromArgb(0, 120, 215);

            totalCount = DatabaseHelper.GetTotalCount();
            todayCount = DatabaseHelper.GetTodayCount();
            currentShift = ShiftHelper.GetCurrentShift();
            lastShift = currentShift;
            mailSentForShift = currentShift;
            shiftCount = DatabaseHelper.GetShiftCount(currentShift);
            shiftTarget = DatabaseHelper.GetShiftTarget(currentShift);

            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";
            lblShift.Text = "Shift: " + currentShift;

            UpdateProgress();
            LoadParts();
            LoadDispatchTokens();

            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.Settings.IsZoomControlEnabled = false;
            webView21.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            string js = @"
(function(){
    let zoomed=false,isPanning=false,startX=0,startY=0;
    document.addEventListener('wheel',function(e){
        if(e.ctrlKey){e.preventDefault();
        let c=parseFloat(document.body.style.zoom)||1.0;
        let n=e.deltaY<0?c+0.1:c-0.1;
        document.body.style.zoom=Math.min(Math.max(n,0.5),3.0).toFixed(1);}
    },{passive:false});
    document.addEventListener('dblclick',function(){zoomed=!zoomed;document.body.style.zoom=zoomed?'1.5':'1.0';});
    document.addEventListener('mousedown',function(e){if(e.button===2){isPanning=true;startX=e.clientX+window.scrollX;startY=e.clientY+window.scrollY;e.preventDefault();}});
    document.addEventListener('mousemove',function(e){if(isPanning)window.scrollTo(startX-e.clientX,startY-e.clientY);});
    document.addEventListener('mouseup',function(e){if(e.button===2)isPanning=false;});
    document.addEventListener('contextmenu',function(e){e.preventDefault();});
})();";
            webView21.CoreWebView2.DOMContentLoaded += async (s2, e2) =>
                await webView21.CoreWebView2.ExecuteScriptAsync(js);

            this.BeginInvoke(new Action(() => LoadPDF()));
            txtScan.Focus();
        }

        // ── dispatch tokens ──────────────────────────────────────────────

        public void LoadDispatchTokens()
        {
            flpTokens.Controls.Clear();
            var orders = DatabaseHelper.GetDispatchOrders();

            foreach (var o in orders)
            {
                var card = BuildTokenCard(o);
                flpTokens.Controls.Add(card);
            }

            // If active order still exists refresh its display
            if (activeOrder != null)
            {
                var refreshed = DatabaseHelper.GetDispatchOrder(activeOrder.OrderNo);
                if (refreshed != null) activeOrder = refreshed;
            }
        }

        private Panel BuildTokenCard(DispatchOrder o)
        {
            Color borderCol = GetDueColour(o.DueDate, o.Status);
            bool isActive = activeOrder?.OrderNo == o.OrderNo;
            bool isDone   = o.Status == "Done";

            var card = new Panel
            {
                Width  = 230,
                Height = isDone ? 175 : 155,
                Margin = new Padding(6),
                Cursor = Cursors.Hand,
                Tag    = o.OrderNo
            };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var bg  = new SolidBrush(isActive
                    ? Color.FromArgb(235, 248, 255) : Color.White);
                using var pen = new Pen(borderCol, isActive ? 3 : 2);
                g.FillRoundedRectangle(bg, 1, 1, card.Width - 2, card.Height - 2, 10);
                g.DrawRoundedRectangle(pen, 1, 1, card.Width - 2, card.Height - 2, 10);
            };

            Color badgeCol = o.Status switch
            {
                "Done"       => Color.FromArgb(60, 160, 60),
                "InProgress" => Color.FromArgb(0, 120, 215),
                _            => Color.FromArgb(150, 150, 150)
            };

            var lblOrder = new Label
            {
                Text      = o.OrderNo,
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location  = new Point(10, 10),
                AutoSize  = true
            };
            var lblCustomer = new Label
            {
                Text      = o.CustomerName,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location  = new Point(10, 28),
                AutoSize  = true
            };
            var lblPart = new Label
            {
                Text      = o.PartName,
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location  = new Point(10, 46),
                AutoSize  = true
            };
            var lblQty = new Label
            {
                Text      = $"Qty: {o.QtyScanned} / {o.QtyOrdered}",
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location  = new Point(10, 64),
                AutoSize  = true
            };
            var lblDue = new Label
            {
                Text      = $"Due: {o.DueDate[..10]}",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = borderCol,
                Location  = new Point(10, 82),
                AutoSize  = true
            };

            // Mini progress bar
            var pb = new ProgressBar
            {
                Location = new Point(10, 100),
                Size     = new Size(210, 8),
                Minimum  = 0,
                Maximum  = Math.Max(o.QtyOrdered, 1),
                Value    = Math.Min(o.QtyScanned, o.QtyOrdered),
                Style    = ProgressBarStyle.Continuous
            };

            var lblStatus = new Label
            {
                Text      = o.Status,
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = badgeCol,
                Location  = new Point(10, 114),
                AutoSize  = true
            };

            card.Controls.AddRange(new Control[]
                { lblOrder, lblCustomer, lblPart, lblQty, lblDue, pb, lblStatus });

            // Reprint button — only for completed orders
            if (isDone)
            {
                var btnReprint = new Button
                {
                    Text      = "🖨 Reprint Label",
                    Location  = new Point(10, 134),
                    Size      = new Size(210, 28),
                    Font      = new Font("Segoe UI", 8F, FontStyle.Bold),
                    BackColor = Color.FromArgb(220, 235, 255),
                    ForeColor = Color.FromArgb(0, 70, 180),
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand
                };
                btnReprint.FlatAppearance.BorderColor = Color.FromArgb(150, 190, 255);
                btnReprint.Click += (s, e) =>
                {
                    currentInspector = txtInspector.Text.Trim();
                    using var dlg = new LabelPrintDialog(
                        o, CurrentUser, currentInspector, printerShareName, baseFolder);
                    dlg.ShowDialog(this);
                };
                card.Controls.Add(btnReprint);
            }

            // Click to select (only non-done orders)
            if (!isDone)
            {
                EventHandler clickHandler = (s, e) => SelectOrder(o.OrderNo);
                card.Click += clickHandler;
                foreach (Control c in card.Controls) c.Click += clickHandler;
            }

            return card;
        }

        private void SelectOrder(string orderNo)
        {
            var order = DatabaseHelper.GetDispatchOrder(orderNo);
            if (order == null) return;

            if (order.Status == "Done")
            {
                MessageBox.Show($"Order {orderNo} is already completed.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Release previous lock
            if (activeOrder != null && activeOrder.OrderNo != orderNo)
                DatabaseHelper.UnlockDispatchOrder(activeOrder.OrderNo);

            // Check if locked by another operator
            if (!string.IsNullOrEmpty(order.LockedBy) && order.LockedBy != CurrentUser)
            {
                MessageBox.Show($"Order {orderNo} is currently being processed by {order.LockedBy}.",
                    "Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DatabaseHelper.LockDispatchOrder(orderNo, CurrentUser);
            activeOrder = order;

            // Update active order panel
            lblActiveOrder.Text = $"Order: {order.OrderNo}";
            lblActiveCustomer.Text = $"Customer: {order.CustomerName}";
            lblActivePart.Text = $"Part: {order.PartName}";
            lblActiveQty.Text = $"{order.QtyScanned} / {order.QtyOrdered} packed";
            progressDispatch.Maximum = Math.Max(order.QtyOrdered, 1);
            progressDispatch.Value = Math.Min(order.QtyScanned, order.QtyOrdered);

            SetDispatchStatus("ORDER SELECTED — Start scanning", Color.FromArgb(0, 120, 215));
            LoadDispatchTokens(); // refresh cards to show active highlight
            txtScan.Focus();
        }

        // ── scan ─────────────────────────────────────────────────────────

        private void txtScan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtScan.Text.Length == 0) scanTimer.Restart();
            lastKeystroke = DateTime.Now;
        }

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            string barcode = txtScan.Text.Trim();
            long elapsed = scanTimer.ElapsedMilliseconds;
            txtScan.Clear();
            scanTimer.Reset();

            if (string.IsNullOrEmpty(barcode)) { txtScan.Focus(); return; }

            // ── Inspector check (compulsory) ─────────────────────────────
            currentInspector = txtInspector.Text.Trim();
            if (string.IsNullOrEmpty(currentInspector))
            {
                SetDispatchStatus(
                    "⚠ Enter Inspector Name first before scanning!",
                    Color.FromArgb(200, 80, 0));
                txtInspector.BackColor = Color.FromArgb(255, 220, 200);
                txtInspector.Focus();
                return;
            }
            txtInspector.BackColor = Color.FromArgb(240, 255, 230); // valid green-tint

            // ── Mode: Dispatch scan ──────────────────────────────────────
            if (activeOrder != null)
            {
                ProcessDispatchScan(barcode, elapsed);
                txtScan.Focus();
                return;
            }

            // ── Mode: EOL label printing (existing flow) ─────────────────
            bool isReprint = false;
            string reprintReason = "";

            if (DatabaseHelper.IsDuplicate(barcode))
            {
                var result = MessageBox.Show(
                    $"Barcode '{barcode}' already printed!\nDo you want to REPRINT?",
                    "Duplicate Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) { txtScan.Focus(); return; }
                reprintReason = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter reprint reason:", "Reprint Reason");
                if (string.IsNullOrWhiteSpace(reprintReason)) { txtScan.Focus(); return; }
                isReprint = true;
            }

            currentShift = ShiftHelper.GetCurrentShift();
            lblShift.Text = "Shift: " + currentShift;

            PrintLabel(barcode);
            SaveToCsv(barcode, isReprint, reprintReason);
            DatabaseHelper.SaveScanLog(barcode, cmbPart.Text,
                CurrentUser, currentShift, isReprint, reprintReason,
                inspector: currentInspector);

            totalCount++; todayCount++; shiftCount++;
            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";
            UpdateProgress();

            lblStatus.Text = isReprint ? "REPRINT" : "PRINTED";
            lblStatus.ForeColor = isReprint ? Color.Orange : Color.Green;

            lstStatus.Items.Insert(0,
                $"[{DateTime.Now:HH:mm:ss}] {barcode} | {cmbPart.Text} | {CurrentUser} | {currentShift} | Insp:{currentInspector}"
                + (isReprint ? " | REPRINT" : ""));

            PlayBeep(isReprint);
            txtScan.Focus();
        }

        private void ProcessDispatchScan(string barcode, long elapsedMs)
        {
            if (activeOrder == null) return;

            // Speed check — reject manual typing
            if (barcode.Length > 1 && elapsedMs > 500)
            {
                SetDispatchStatus("Too slow — looks like manual typing. Rescan.", Color.Orange);
                PlayBeep(true);
                return;
            }

            // Duplicate within this order
            var scans = DatabaseHelper.GetDispatchScans(activeOrder.OrderNo);
            if (scans.Any(s => s.Barcode == barcode && s.Result == "OK"))
            {
                SetDispatchStatus($"DUPLICATE — already scanned: {barcode}", Color.Orange);
                PlayBeep(true);
                DatabaseHelper.SaveDispatchScan(activeOrder.OrderNo, barcode, CurrentUser, "Duplicate");
                return;
            }

            // Part match check
            bool matched = IsPartMatch(barcode, activeOrder.QRReference);
            if (!matched)
            {
                SetDispatchStatus($"WRONG PART LOADED — Expected ref: {activeOrder.QRReference}", Color.Red);
                PlayBeep(true);
                DatabaseHelper.SaveDispatchScan(activeOrder.OrderNo, barcode, CurrentUser, "WrongPart");
                lstStatus.Items.Insert(0,
                    $"[{DateTime.Now:HH:mm:ss}] WRONG PART | Order:{activeOrder.OrderNo} | Scanned:{barcode}");
                return;
            }

            // Qty exceeded
            if (activeOrder.QtyScanned >= activeOrder.QtyOrdered)
            {
                SetDispatchStatus("Order quantity already complete!", Color.Orange);
                PlayBeep(true);
                return;
            }

            // ── SUCCESS ──────────────────────────────────────────────────
            DatabaseHelper.IncrementDispatchScan(activeOrder.OrderNo);
            DatabaseHelper.SaveDispatchScan(activeOrder.OrderNo, barcode, CurrentUser, "OK");

            // Refresh active order from DB
            activeOrder = DatabaseHelper.GetDispatchOrder(activeOrder.OrderNo)!;

            int remaining = activeOrder.QtyOrdered - activeOrder.QtyScanned;
            SetDispatchStatus($"OK — {remaining} remaining", Color.Green);
            PlayBeep(false);

            // Update active panel
            lblActiveQty.Text = $"{activeOrder.QtyScanned} / {activeOrder.QtyOrdered} packed";
            progressDispatch.Value = Math.Min(activeOrder.QtyScanned, activeOrder.QtyOrdered);

            lstStatus.Items.Insert(0,
                $"[{DateTime.Now:HH:mm:ss}] OK | {barcode} | {activeOrder.OrderNo} | {remaining} left");

            LoadDispatchTokens(); // refresh card progress

            // ── ORDER COMPLETE ────────────────────────────────────────────
            if (activeOrder.QtyScanned >= activeOrder.QtyOrdered)
                CompleteOrder();
        }

        private bool IsPartMatch(string scanned, string qrRef)
        {
            if (string.IsNullOrEmpty(qrRef)) return false;
            if (scanned == qrRef) return true;
            if (scanned.Contains(qrRef)) return true;
            return false;
        }

        private void CompleteOrder()
        {
            if (activeOrder == null) return;

            DatabaseHelper.CompleteDispatchOrder(activeOrder.OrderNo);
            activeOrder = DatabaseHelper.GetDispatchOrder(activeOrder.OrderNo)!;

            SetDispatchStatus($"ORDER COMPLETE — {activeOrder.OrderNo}", Color.Green);
            SystemSounds.Exclamation.Play();

            // Capture completed order before clearing activeOrder
            var completedOrder = activeOrder;

            activeOrder = null;
            lblActiveOrder.Text    = "No order selected";
            lblActiveCustomer.Text = "";
            lblActivePart.Text     = "";
            lblActiveQty.Text      = "";
            progressDispatch.Value = 0;

            // Export CSV for this order
            string csvPath = ExportOrderCsv(completedOrder);

            // Email with CSV
            EmailHelper.SendEmailAsync(csvPath,
                $"Dispatch Complete — {completedOrder.OrderNo} — {completedOrder.CustomerName}");

            MessageBox.Show(
                $"Order {completedOrder.OrderNo} completed!\n" +
                $"Customer: {completedOrder.CustomerName}\n" +
                $"Qty dispatched: {completedOrder.QtyScanned}\n\n" +
                "Please fill in label details to print. Report emailed.",
                "Order Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Open label print dialog
            currentInspector = txtInspector.Text.Trim();
            using var printDlg = new LabelPrintDialog(
                completedOrder, CurrentUser, currentInspector,
                printerShareName, baseFolder);
            printDlg.ShowDialog(this);

            LoadDispatchTokens();
            txtScan.Focus();
        }

        // ── CSV export per order ─────────────────────────────────────────

        private string ExportOrderCsv(DispatchOrder order)
        {
            string fileName = $"dispatch_{order.OrderNo}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string filePath = Path.Combine(baseFolder, fileName);

            var sb = new StringBuilder();
            sb.AppendLine("OrderNo,CreatedDate,DueDate,CustomerName,PartName,QRReference," +
                          "QtyOrdered,QtyScanned,QtyPending,Status,CompletedAt");
            sb.AppendLine(
                $"{order.OrderNo},{order.CreatedDate},{order.DueDate}," +
                $"{order.CustomerName},{order.PartName},{order.QRReference}," +
                $"{order.QtyOrdered},{order.QtyScanned},{order.QtyPending}," +
                $"{order.Status},{order.CompletedAt}");

            sb.AppendLine();
            sb.AppendLine("Sr,Barcode,ScanTime,Operator,Result");
            int sr = 1;
            foreach (var scan in DatabaseHelper.GetDispatchScans(order.OrderNo))
            {
                sb.AppendLine($"{sr},{scan.Barcode},{scan.ScanTime},{scan.Operator},{scan.Result}");
                sr++;
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            return filePath;
        }

        // ── shift CSV (existing) ─────────────────────────────────────────

        public string GetCsvPath()
        {
            string fileName = $"log_{currentShift}_{DateTime.Now:yyyy-MM-dd}.csv";
            return Path.Combine(baseFolder, fileName);
        }

        private void SaveToCsv(string barcode, bool isReprint = false, string reason = "")
        {
            try
            {
                string file = GetCsvPath();
                if (!File.Exists(file))
                    File.WriteAllText(file, "SrNo,DateTime,Barcode,Part,User,Shift,Reprint,Reason\n");
                int sr = File.ReadAllLines(file).Length;
                using var sw = new StreamWriter(file, true);
                sw.WriteLine($"{sr},{DateTime.Now:yyyy-MM-dd HH:mm:ss},{barcode}," +
                             $"{cmbPart.Text},{CurrentUser},{currentShift}," +
                             $"{(isReprint ? "YES" : "NO")},{reason}");
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log", $"[{DateTime.Now}] CSV Error: {ex.Message}\n");
            }
        }

        // ── dispatch label print ─────────────────────────────────────────

        private void PrintDispatchLabel(DispatchOrder order)
        {
            try
            {
                string prnContent = DatabaseHelper.GetCustomerPrnContent(order.CustomerName);

                if (string.IsNullOrWhiteSpace(prnContent))
                {
                    MessageBox.Show(
                        $"No PRN configured for customer '{order.CustomerName}'!\n" +
                        "Go to Admin → Customer PRN to set up the label format.",
                        "PRN Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Strip comments
                prnContent = string.Join("\r\n",
                    prnContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                              .Where(l => !l.TrimStart().StartsWith("//"))) + "\r\n";

                // Replace dispatch tokens
                prnContent = prnContent
                    .Replace("{OrderNo}", order.OrderNo)
                    .Replace("{CustomerName}", order.CustomerName)
                    .Replace("{PartName}", order.PartName)
                    .Replace("{QtyOrdered}", order.QtyOrdered.ToString())
                    .Replace("{QtyScanned}", order.QtyScanned.ToString())
                    .Replace("{DueDate}", order.DueDate)
                    .Replace("{CompletedAt}", order.CompletedAt)
                    .Replace("{CreatedDate}", order.CreatedDate);

                // Also support {MULTILINE_TEXT:...} tokens
                prnContent = ResolveMultilineTokens(prnContent, order.OrderNo);

                string tempPrn = Path.Combine(baseFolder, "dispatch_label.prn");
                File.WriteAllText(tempPrn, prnContent, Encoding.ASCII);

                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c copy /b \"{tempPrn}\" \"\\\\localhost\\{printerShareName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var proc = Process.Start(psi);
                proc?.WaitForExit(3000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dispatch Label Print Error: " + ex.Message);
                File.AppendAllText("error.log",
                    $"[{DateTime.Now}] Dispatch Label Error: {ex.Message}\n");
            }
        }

        // ── existing EOL label print (unchanged) ────────────────────────

        private void PrintLabel(string barcode)
        {
            try
            {
                if (!Directory.Exists(baseFolder)) Directory.CreateDirectory(baseFolder);
                string partName = cmbPart.Text.Trim();
                string prnContent = DatabaseHelper.GetPrnContent(partName);

                if (string.IsNullOrWhiteSpace(prnContent))
                {
                    string filePath = DatabaseHelper.GetPrnPath(partName);
                    if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                        prnContent = File.ReadAllText(filePath, Encoding.ASCII);
                }

                if (!string.IsNullOrWhiteSpace(prnContent))
                    prnContent = string.Join("\r\n",
                        prnContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                                  .Where(l => !l.TrimStart().StartsWith("//"))) + "\r\n";

                if (string.IsNullOrWhiteSpace(prnContent?.Replace("\r\n", "").Trim()))
                {
                    MessageBox.Show(
                        $"No PRN configured for part '{partName}'!\nPlease set PRN in Admin → PRN Editor.",
                        "PRN Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                prnContent = prnContent!
                    .Replace("{barcode}", barcode)
                    .Replace("{PartName}", partName)
                    .Replace("{serialNumber}", serialNumber.ToString());

                prnContent = ResolveMultilineTokens(prnContent, barcode);

                string tempPrn = Path.Combine(baseFolder, "active_label.prn");
                File.WriteAllText(tempPrn, prnContent, Encoding.ASCII);

                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c copy /b \"{tempPrn}\" \"\\\\localhost\\{printerShareName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process.Start(psi)?.WaitForExit(3000);

                serialNumber++;
                DatabaseHelper.SaveSerial(serialNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Error: " + ex.Message);
                File.AppendAllText("error.log", $"[{DateTime.Now}] Print Error: {ex.Message}\n");
            }
        }

        // ── multiline PRN helpers (unchanged from before) ────────────────

        private static IEnumerable<string> SplitIntoChunks(string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }

        private static string BuildMultilineTextCommands(
            string fullString, int x, int startY, int lineHeight,
            string font, int rotation, int xMul, int yMul, int chunkSize)
        {
            var sb = new StringBuilder();
            int y = startY;
            var chunks = SplitIntoChunks(fullString, chunkSize).ToList();
            for (int i = 0; i < chunks.Count; i++)
            {
                string chunk = chunks[i];
                int lineX = x;
                if (i == chunks.Count - 1 && chunk.Length < chunkSize)
                {
                    int charWidth = (font == "1" ? 8 : font == "2" ? 10 : 12) * xMul;
                    int fullW = chunkSize * charWidth;
                    int lastW = chunk.Length * charWidth;
                    lineX = x + (fullW - lastW) / 2;
                }
                sb.Append($"TEXT {lineX},{y},\"{font}\",{rotation},{xMul},{yMul},\"{chunk}\"\r\n");
                y += lineHeight;
            }
            return sb.ToString();
        }

        private static string ResolveMultilineTokens(string prnContent, string value)
        {
            var pattern = new Regex(@"\{MULTILINE_TEXT:([^}]+)\}", RegexOptions.IgnoreCase);
            return pattern.Replace(prnContent, match =>
            {
                var args = match.Groups[1].Value
                    .Split(',').Select(p => p.Trim().Split('='))
                    .Where(p => p.Length == 2)
                    .ToDictionary(p => p[0].Trim().ToUpper(), p => p[1].Trim(),
                        StringComparer.OrdinalIgnoreCase);
                int x = args.TryGetValue("X", out var vx) && int.TryParse(vx, out var ix) ? ix : 10;
                int y = args.TryGetValue("Y", out var vy) && int.TryParse(vy, out var iy) ? iy : 50;
                int lh = args.TryGetValue("LH", out var vlh) && int.TryParse(vlh, out var ilh) ? ilh : 25;
                int cs = args.TryGetValue("CS", out var vcs) && int.TryParse(vcs, out var ics) ? ics : 10;
                int rot = args.TryGetValue("ROT", out var vr) && int.TryParse(vr, out var ir) ? ir : 0;
                int xm = args.TryGetValue("XM", out var vxm) && int.TryParse(vxm, out var ixm) ? ixm : 1;
                int ym = args.TryGetValue("YM", out var vym) && int.TryParse(vym, out var iym) ? iym : 1;
                string font = args.TryGetValue("FONT", out var vf) ? vf : "3";
                return BuildMultilineTextCommands(value, x, y, lh, font, rot, xm, ym, cs);
            });
        }

        // ── helpers ──────────────────────────────────────────────────────

        private void SetDispatchStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private Color GetDueColour(string dueDateStr, string status)
        {
            if (status == "Done") return Color.FromArgb(60, 160, 60);
            if (!DateTime.TryParse(dueDateStr, out DateTime due)) return Color.Gray;
            int daysLeft = (due.Date - DateTime.Today).Days;
            int yellow = int.TryParse(DatabaseHelper.GetConfig("YellowDaysBeforeDue"), out int y) ? y : 1;
            int red = int.TryParse(DatabaseHelper.GetConfig("RedDaysBeforeDue"), out int r) ? r : 0;
            if (daysLeft <= red) return Color.FromArgb(210, 50, 50);
            if (daysLeft <= yellow) return Color.FromArgb(200, 140, 0);
            return Color.FromArgb(30, 130, 30);
        }

        private void PlayBeep(bool isError)
        {
            try { if (isError) SystemSounds.Exclamation.Play(); else SystemSounds.Beep.Play(); }
            catch { }
        }

        private void SendShiftReport()
        {
            string file = GetCsvPath();
            if (File.Exists(file))
                EmailHelper.SendEmailAsync(file,
                    $"Shift {currentShift} Report — {DateTime.Now:dd-MM-yyyy}");
        }

        // ── parts / PDF / progress (unchanged) ──────────────────────────

        private void LoadParts()
        {
            string previousPart = cmbPart.SelectedItem?.ToString() ?? "";
            cmbPart.Items.Clear();
            foreach (var part in DatabaseHelper.GetParts()) cmbPart.Items.Add(part);
            if (!string.IsNullOrEmpty(previousPart) && cmbPart.Items.Contains(previousPart))
                cmbPart.SelectedItem = previousPart;
            else if (cmbPart.Items.Count > 0)
                cmbPart.SelectedIndex = 0;
        }

        public void LoadPDF()
        {
            try
            {
                if (webView21.CoreWebView2 == null) return;
                string path = DatabaseHelper.GetPdfPath(cmbPart.Text);
                if (string.IsNullOrEmpty(path)) path = DatabaseHelper.GetPdfPath();
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    webView21.CoreWebView2.Navigate(
                        new Uri(path).AbsoluteUri + "?v=" + DateTime.Now.Ticks);
                else
                    webView21.NavigateToString(
                        "<h2 style='font-family:Segoe UI;color:gray;text-align:center;margin-top:60px'>No SOP Found</h2>");
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log", $"[{DateTime.Now}] PDF Error: {ex.Message}\n");
            }
        }

        private void UpdateProgress()
        {
            if (shiftTarget > 0)
            {
                int pct = Math.Min((shiftCount * 100) / shiftTarget, 100);
                progressShift.Value = pct;
                lblProgress.Text = $"Shift: {shiftCount} / {shiftTarget} ({pct}%)";
                progressShift.ForeColor = pct >= 100 ? Color.Green : Color.FromArgb(0, 120, 215);
            }
            else
            {
                progressShift.Value = 0;
                lblProgress.Text = $"Shift: {shiftCount} (No target set)";
            }
        }

        // ── buttons ──────────────────────────────────────────────────────

        private void btnClear_Click(object sender, EventArgs e) => lstStatus.Items.Clear();

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset today's count?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                totalCount = 0; todayCount = 0; shiftCount = 0;
                lblTotal.Text = "Total: 0"; lblToday.Text = "Today: 0";
                UpdateProgress();
            }
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (CurrentUser != "admin")
            {
                MessageBox.Show("Only admin allowed!", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var admin = new AdminForm();
            admin.ShowDialog();
            LoadParts(); LoadPDF();
            printerShareName = DatabaseHelper.GetConfig("PrinterShareName");
            shiftTarget = DatabaseHelper.GetShiftTarget(currentShift);
            UpdateProgress();
            LoadDispatchTokens();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Release any active dispatch lock
                if (activeOrder != null)
                {
                    DatabaseHelper.UnlockDispatchOrder(activeOrder.OrderNo);
                    activeOrder = null;
                }

                // Export shift dispatch summary CSV
                ExportShiftDispatchCsv();

                EmailHelper.SendEmailAsync(GetCsvPath(),
                    $"Logout Report — {CurrentUser} — {DateTime.Now:dd-MM-yyyy HH:mm}");

                timerClock.Stop();
                LoginForm login = new LoginForm();
                this.Hide();
                if (login.ShowDialog() == DialogResult.OK)
                {
                    var newForm = new Form1(login.LoggedUser);
                    newForm.Show();
                }
                this.Close();
            }
        }

        private void ExportShiftDispatchCsv()
        {
            try
            {
                var orders = DatabaseHelper.GetDispatchOrders();
                if (!orders.Any()) return;

                string fileName = $"dispatch_shift_{currentShift}_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
                string filePath = Path.Combine(baseFolder, fileName);

                var sb = new StringBuilder();
                sb.AppendLine("OrderNo,CustomerName,PartName,QtyOrdered,QtyScanned,QtyPending,Status,DueDate,CompletedAt");
                foreach (var o in orders)
                    sb.AppendLine($"{o.OrderNo},{o.CustomerName},{o.PartName}," +
                                  $"{o.QtyOrdered},{o.QtyScanned},{o.QtyPending}," +
                                  $"{o.Status},{o.DueDate},{o.CompletedAt}");

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

                EmailHelper.SendEmailAsync(filePath,
                    $"Shift {currentShift} Dispatch Summary — {DateTime.Now:dd-MM-yyyy}");
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log",
                    $"[{DateTime.Now}] Shift dispatch CSV error: {ex.Message}\n");
            }
        }
        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (activeOrder == null) return;
            if (MessageBox.Show($"Release order {activeOrder.OrderNo}?\nProgress will be saved.",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.UnlockDispatchOrder(activeOrder.OrderNo);
                activeOrder = null;
                lblActiveOrder.Text = "No order selected";
                lblActiveCustomer.Text = "";
                lblActivePart.Text = "";
                lblActiveQty.Text = "";
                progressDispatch.Value = 0;
                SetDispatchStatus("READY", System.Drawing.Color.FromArgb(0, 120, 215));
                LoadDispatchTokens();
                txtScan.Focus();
            }
        }

        private void btnRefreshTokens_Click(object sender, EventArgs e)
        {
            LoadDispatchTokens();
            txtScan.Focus();
        }

        private void btnOpenCsv_Click(object sender, EventArgs e) { }
        private void btnTestMail_Click(object sender, EventArgs e) { }

        private void cmbPart_SelectedIndexChanged(object sender, EventArgs e) => LoadPDF();

        private void btnZoomIn_Click(object sender, EventArgs e) =>
            webView21.ZoomFactor = Math.Min(webView21.ZoomFactor + 0.1, 3.0);

        private void btnZoomOut_Click(object sender, EventArgs e) =>
            webView21.ZoomFactor = Math.Max(webView21.ZoomFactor - 0.1, 0.5);

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e) { }
    }


    // ── Graphics extension for rounded rectangles ─────────────────────────
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this System.Drawing.Graphics g,
            Brush brush, float x, float y, float w, float h, float r)
        {
            using var path = GetRoundedPath(x, y, w, h, r);
            g.FillPath(brush, path);
        }

        public static void DrawRoundedRectangle(this System.Drawing.Graphics g,
            Pen pen, float x, float y, float w, float h, float r)
        {
            using var path = GetRoundedPath(x, y, w, h, r);
            g.DrawPath(pen, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath GetRoundedPath(
            float x, float y, float w, float h, float r)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(x, y, r * 2, r * 2, 180, 90);
            path.AddArc(x + w - r * 2, y, r * 2, r * 2, 270, 90);
            path.AddArc(x + w - r * 2, y + h - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(x, y + h - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

    }
}