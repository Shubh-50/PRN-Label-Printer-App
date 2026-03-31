using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public partial class Form1 : Form
    {
        private string printerShareName = "TSC_TE244";

        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "BarcodeApp");

        private string prnPath = "";
        private int totalCount = 0;
        private int todayCount = 0;
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
            prnPath = Path.Combine(baseFolder, "label.prn");
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

            lblUser.Text = "User: " + currentUser;
            lblStatus.Text = "READY";

            totalCount = DatabaseHelper.GetTotalCount();
            todayCount = DatabaseHelper.GetTodayCount();

            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";

            LoadParts();

            currentShift = ShiftHelper.GetCurrentShift();
            lastShift = currentShift;
            lblShift.Text = "Shift: " + currentShift;

            txtScan.Focus();

            await webView21.EnsureCoreWebView2Async();
            this.BeginInvoke(new Action(() => LoadPDF()));
        }

        // ================= LOAD PARTS =================

        private void LoadParts()
        {
            cmbPart.Items.Clear();
            var parts = DatabaseHelper.GetParts();
            foreach (var part in parts)
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

                string partName = cmbPart.Text;
                string path = DatabaseHelper.GetPdfPath(partName);

                if (string.IsNullOrEmpty(path))
                    path = DatabaseHelper.GetPdfPath();

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    string uri = new Uri(path).AbsoluteUri;
                    webView21.CoreWebView2.Navigate(uri + "?v=" + DateTime.Now.Ticks);
                }
                else
                {
                    webView21.NavigateToString("<h2 style='font-family:Segoe UI;color:gray;text-align:center;margin-top:50px'>No SOP Found</h2>");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log", $"[{DateTime.Now}] PDF Error: {ex.Message}\n");
            }
        }

        // ================= SHIFT MAIL =================

        private void SendShiftReport()
        {
            string file = GetCsvPath();
            if (File.Exists(file))
                EmailHelper.SendEmailAsync(file,
                    $"Shift Report - {currentShift} - {DateTime.Now:dd-MM-yyyy}");
        }

        // ================= SCANNER =================

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            string barcode = txtScan.Text.Trim();
            txtScan.Clear();
            txtScan.Focus();

            if (string.IsNullOrEmpty(barcode)) return;

            // 🔥 Duplicate check
            if (DatabaseHelper.IsDuplicate(barcode))
            {
                var result = MessageBox.Show(
                    $"Barcode '{barcode}' already printed!\nDo you want to REPRINT?",
                    "Duplicate Detected",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes) return;
            }

            currentShift = ShiftHelper.GetCurrentShift();
            lblShift.Text = "Shift: " + currentShift;

            PrintLabel(barcode);
            SaveToCsv(barcode);
            DatabaseHelper.SaveScanLog(barcode, cmbPart.Text, currentUser, currentShift);

            totalCount++;
            todayCount++;

            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";
            lblStatus.Text = "✅ PRINTED";
            lblStatus.ForeColor = System.Drawing.Color.Green;

            lstStatus.Items.Insert(0,
                $"[{DateTime.Now:HH:mm:ss}] {barcode} | {cmbPart.Text} | {currentUser} | {currentShift}");
        }

        // ================= CSV =================

        private string GetCsvPath()
        {
            string fileName = $"log_{currentShift}_{DateTime.Now:yyyy-MM-dd}.csv";
            return Path.Combine(baseFolder, fileName);
        }

        private void SaveToCsv(string barcode)
        {
            try
            {
                string file = GetCsvPath();

                if (!File.Exists(file))
                    File.WriteAllText(file, "SrNo,DateTime,Barcode,Part,User,Shift\n");

                int sr = File.ReadAllLines(file).Length;

                using (var sw = new StreamWriter(file, true))
                {
                    sw.WriteLine(
                        $"{sr},{DateTime.Now:yyyy-MM-dd HH:mm:ss},{barcode},{cmbPart.Text},{currentUser},{currentShift}");
                }
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

                string prn = $@"SIZE 24 mm,8 mm
GAP 1 mm,0 mm
CLS
QRCODE 5,5,L,3,A,0,""{barcode}""
TEXT 45,5,""2"",0,1,1,""{barcode}""
TEXT 45,18,""2"",0,1,1,""{cmbPart.Text}""
TEXT 45,31,""2"",0,1,1,""{serialNumber}""
PRINT 1
";
                File.WriteAllText(prnPath, prn);

                var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c copy /b \"{prnPath}\" \"\\\\localhost\\{printerShareName}\"",
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
                totalCount = 0;
                todayCount = 0;
                lblTotal.Text = "Total: 0";
                lblToday.Text = "Today: 0";
            }
        }

        private void btnOpenCsv_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", baseFolder);
        }

        private void btnTestMail_Click(object sender, EventArgs e)
        {
            string file = GetCsvPath();
            if (!File.Exists(file))
            {
                MessageBox.Show("No CSV found for today's shift ❌");
                return;
            }
            EmailHelper.SendEmailAsync(file, "Test Report");
            MessageBox.Show("Email sending in background ✅");
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (currentUser != "admin")
            {
                MessageBox.Show("Only admin allowed ❌");
                return;
            }
            AdminForm admin = new AdminForm();
            admin.ShowDialog();
            LoadParts();
            LoadPDF();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string file = GetCsvPath();
                if (File.Exists(file))
                    EmailHelper.SendEmailAsync(file,
                        $"Logout Report - {currentUser} - {DateTime.Now:dd-MM-yyyy HH:mm}");

                timerClock.Stop();

                LoginForm login = new LoginForm();
                this.Hide();
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Form1 newForm = new Form1(login.LoggedUser);
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
    }
}