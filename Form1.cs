using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public partial class Form1 : Form
    {
        private string printerShareName = "";
        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeApp");
        private int totalCount = 0;
        private int todayCount = 0;
        private int shiftCount = 0;
        private int shiftTarget = 0;
        private int serialNumber = 500;
        private string currentUser = "";
        private string currentShift = "";
        private string lastShift = "";
        private bool shiftMailSent = false;
        private System.Windows.Forms.Timer timerClock = new System.Windows.Forms.Timer();

        public Form1(string user)
        {
            InitializeComponent();
            currentUser = user;
            printerShareName = DatabaseHelper.GetConfig("PrinterShareName");
            serialNumber = DatabaseHelper.GetSerial();
            timerClock.Interval = 1000;
            timerClock.Tick += TimerClock_Tick;
            timerClock.Start();
        }

        private void TimerClock_Tick(object? sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            string newShift = ShiftHelper.GetCurrentShift();
            if (newShift != lastShift)
            {
                lastShift = newShift;
                lblShift.Text = "Shift: " + newShift;
                shiftMailSent = false;
                shiftCount = DatabaseHelper.GetShiftCount(newShift);
                shiftTarget = DatabaseHelper.GetShiftTarget(newShift);
                UpdateProgress();
            }
            if (!shiftMailSent)
            {
                SendShiftReport();
                shiftMailSent = true;
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            this.Text = $"Packaging EOL System — {DatabaseHelper.AppVersion}";
            lblUser.Text = "User: " + currentUser;
            lblStatus.Text = "READY";
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);

            totalCount = DatabaseHelper.GetTotalCount();
            todayCount = DatabaseHelper.GetTodayCount();
            currentShift = ShiftHelper.GetCurrentShift();
            lastShift = currentShift;
            shiftCount = DatabaseHelper.GetShiftCount(currentShift);
            shiftTarget = DatabaseHelper.GetShiftTarget(currentShift);

            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";
            lblShift.Text = "Shift: " + currentShift;

            UpdateProgress();
            LoadParts();

            txtScan.Focus();

            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.Settings.IsZoomControlEnabled = false;
            webView21.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            // JS: Ctrl+Scroll zoom, double-click toggle, right-click drag pan
            string js = @"
(function(){
    let zoomed = false;
    let isPanning = false, startX = 0, startY = 0;

    document.addEventListener('wheel', function(e){
        if(e.ctrlKey){
            e.preventDefault();
            let current = parseFloat(document.body.style.zoom) || 1.0;
            let next = e.deltaY < 0 ? current + 0.1 : current - 0.1;
            document.body.style.zoom = Math.min(Math.max(next, 0.5), 3.0).toFixed(1);
        }
    }, {passive:false});

    document.addEventListener('dblclick', function(){
        zoomed = !zoomed;
        document.body.style.zoom = zoomed ? '1.5' : '1.0';
    });

    document.addEventListener('mousedown', function(e){
        if(e.button===2){
            isPanning = true;
            startX = e.clientX + window.scrollX;
            startY = e.clientY + window.scrollY;
            e.preventDefault();
        }
    });
    document.addEventListener('mousemove', function(e){
        if(isPanning) window.scrollTo(startX - e.clientX, startY - e.clientY);
    });
    document.addEventListener('mouseup', function(e){
        if(e.button===2) isPanning = false;
    });
    document.addEventListener('contextmenu', function(e){
        e.preventDefault();
    });
})();
";
            webView21.CoreWebView2.DOMContentLoaded += async (s2, e2) =>
                await webView21.CoreWebView2.ExecuteScriptAsync(js);

            this.BeginInvoke(new Action(() => LoadPDF()));
        }

        // ================= PARTS =================

        private void LoadParts()
        {
            cmbPart.Items.Clear();
            foreach (var part in DatabaseHelper.GetParts())
                cmbPart.Items.Add(part);
            if (cmbPart.Items.Count > 0)
                cmbPart.SelectedIndex = 0;
        }

        // ================= PDF =================

        public void LoadPDF()
        {
            try
            {
                if (webView21.CoreWebView2 == null) return;
                string path = DatabaseHelper.GetPdfPath(cmbPart.Text);
                if (string.IsNullOrEmpty(path))
                    path = DatabaseHelper.GetPdfPath();
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

        // ================= PROGRESS =================

        private void UpdateProgress()
        {
            if (shiftTarget > 0)
            {
                int pct = Math.Min((shiftCount * 100) / shiftTarget, 100);
                progressShift.Value = pct;
                lblProgress.Text = $"Shift: {shiftCount} / {shiftTarget} ({pct}%)";
                progressShift.ForeColor = pct >= 100
                    ? System.Drawing.Color.Green
                    : System.Drawing.Color.FromArgb(0, 120, 215);
            }
            else
            {
                progressShift.Value = 0;
                lblProgress.Text = $"Shift: {shiftCount} (No target set)";
            }
        }

        // ================= SCAN =================

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            string barcode = txtScan.Text.Trim();
            txtScan.Clear();
            txtScan.Focus();

            if (string.IsNullOrEmpty(barcode)) return;

            bool isReprint = false;
            string reprintReason = "";

            if (DatabaseHelper.IsDuplicate(barcode))
            {
                var result = MessageBox.Show(
                    $"Barcode '{barcode}' already printed!\nDo you want to REPRINT?",
                    "Duplicate Detected",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;

                reprintReason = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter reprint reason:", "Reprint Reason");
                if (string.IsNullOrWhiteSpace(reprintReason)) return;
                isReprint = true;
            }

            currentShift = ShiftHelper.GetCurrentShift();
            lblShift.Text = "Shift: " + currentShift;

            PrintLabel(barcode);
            SaveToCsv(barcode, isReprint, reprintReason);
            DatabaseHelper.SaveScanLog(barcode, cmbPart.Text,
                currentUser, currentShift, isReprint, reprintReason);

            totalCount++;
            todayCount++;
            shiftCount++;

            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";
            UpdateProgress();

            lblStatus.Text = isReprint ? "REPRINT" : "PRINTED";
            lblStatus.ForeColor = isReprint
                ? System.Drawing.Color.Orange
                : System.Drawing.Color.Green;

            lstStatus.Items.Insert(0,
                $"[{DateTime.Now:HH:mm:ss}] {barcode} | {cmbPart.Text} | {currentUser} | {currentShift}"
                + (isReprint ? " | REPRINT" : ""));

            PlayBeep(isReprint);
        }

        // ================= BEEP =================

        private void PlayBeep(bool isReprint)
        {
            try
            {
                if (isReprint)
                    SystemSounds.Exclamation.Play();
                else
                    SystemSounds.Beep.Play();
            }
            catch { }
        }

        // ================= CSV =================

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
                    File.WriteAllText(file,
                        "SrNo,DateTime,Barcode,Part,User,Shift,Reprint,Reason\n");
                int sr = File.ReadAllLines(file).Length;
                using (var sw = new StreamWriter(file, true))
                    sw.WriteLine(
                        $"{sr},{DateTime.Now:yyyy-MM-dd HH:mm:ss},{barcode}," +
                        $"{cmbPart.Text},{currentUser},{currentShift}," +
                        $"{(isReprint ? "YES" : "NO")},{reason}");
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log", $"[{DateTime.Now}] CSV Error: {ex.Message}\n");
            }
        }

        // ================= PRINT =================

        private void PrintLabel(string barcode)
        {
            try
            {
                if (!Directory.Exists(baseFolder))
                    Directory.CreateDirectory(baseFolder);

                string partName = cmbPart.Text.Trim();
                string prnContent = DatabaseHelper.GetPrnContent(partName);
                string filePath = DatabaseHelper.GetPrnPath(partName);

                if (string.IsNullOrWhiteSpace(prnContent) && File.Exists(filePath))
                    prnContent = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(prnContent))
                    prnContent =
                        "SIZE 20 mm,8 mm\r\n" +
                        "GAP 2 mm,0 mm\r\n" +
                        "SPEED 2\r\n" +
                        "DENSITY 10\r\n" +
                        "DIRECTION 0,0\r\n" +
                        "REFERENCE 0,0\r\n" +
                        "CLS\r\n" +
                        "QRCODE 2,2,L,2,A,0,\"{barcode}\"\r\n" +
                        "TEXT 52,0,\"1\",0,1,1,\"{barcode}\"\r\n" +
                        "TEXT 52,16,\"1\",0,1,1,\"{PartName}\"\r\n" +
                        "TEXT 52,32,\"1\",0,1,1,\"7810326007\"\r\n" +
                        "TEXT 52,48,\"1\",0,1,1,\"{serialNumber}\"\r\n" +
                        "PRINT 1\r\n";

                // DEBUG — DB content check
                File.WriteAllText(Path.Combine(baseFolder, "debug_prn.txt"), prnContent);

                // Replace tokens
                prnContent = prnContent
                    .Replace("{barcode}", barcode)
                    .Replace("{PartName}", partName)
                    .Replace("{serialNumber}", serialNumber.ToString());

                // DEBUG — final content after replace
                File.WriteAllText(Path.Combine(baseFolder, "debug_final.txt"), prnContent);

                string tempPrn = Path.Combine(baseFolder, "active_label.prn");
                File.WriteAllText(tempPrn, prnContent, System.Text.Encoding.ASCII);

                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c copy /b \"{tempPrn}\" \"\\\\localhost\\{printerShareName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                serialNumber++;
                DatabaseHelper.SaveSerial(serialNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Error: " + ex.Message);
                File.AppendAllText("error.log", $"[{DateTime.Now}] Print Error: {ex.Message}\n");
            }
        }

        // ================= SHIFT MAIL =================

        private void SendShiftReport()
        {
            string file = GetCsvPath();
            if (File.Exists(file))
                EmailHelper.SendEmailAsync(file,
                    $"Shift {currentShift} Report — {DateTime.Now:dd-MM-yyyy}");
        }

        // ================= BUTTONS =================

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstStatus.Items.Clear();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset today's count?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                totalCount = 0; todayCount = 0; shiftCount = 0;
                lblTotal.Text = "Total: 0";
                lblToday.Text = "Today: 0";
                UpdateProgress();
            }
        }

        // Moved to AdminForm — kept as stubs so designer doesn't break
        private void btnOpenCsv_Click(object sender, EventArgs e) { }
        private void btnTestMail_Click(object sender, EventArgs e) { }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (currentUser != "admin")
            {
                MessageBox.Show("Only admin allowed!", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var admin = new AdminForm();
            admin.ShowDialog();
            LoadParts();
            LoadPDF();
            printerShareName = DatabaseHelper.GetConfig("PrinterShareName");
            shiftTarget = DatabaseHelper.GetShiftTarget(currentShift);
            UpdateProgress();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                EmailHelper.SendEmailAsync(GetCsvPath(),
                    $"Logout Report — {currentUser} — {DateTime.Now:dd-MM-yyyy HH:mm}");
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

        private void cmbPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPDF();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            webView21.ZoomFactor = Math.Min(webView21.ZoomFactor + 0.1, 3.0);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            webView21.ZoomFactor = Math.Max(webView21.ZoomFactor - 0.1, 0.5);
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
