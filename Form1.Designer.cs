namespace BarcodeBartenderApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelPdf;

        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.ComboBox cmbPart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblToday;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblShift;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.ListBox lstStatus;

        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnOpenCsv;
        private System.Windows.Forms.Button btnTestMail;
        private System.Windows.Forms.Button btnAdmin;
        private System.Windows.Forms.Button btnLogout;

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            panelTop = new System.Windows.Forms.Panel();
            panelPdf = new System.Windows.Forms.Panel();

            txtScan = new System.Windows.Forms.TextBox();
            cmbPart = new System.Windows.Forms.ComboBox();
            lblStatus = new System.Windows.Forms.Label();
            lblTotal = new System.Windows.Forms.Label();
            lblToday = new System.Windows.Forms.Label();
            lblDateTime = new System.Windows.Forms.Label();
            lblShift = new System.Windows.Forms.Label();
            lblUser = new System.Windows.Forms.Label();
            lstStatus = new System.Windows.Forms.ListBox();

            btnClear = new System.Windows.Forms.Button();
            btnReset = new System.Windows.Forms.Button();
            btnOpenCsv = new System.Windows.Forms.Button();
            btnTestMail = new System.Windows.Forms.Button();
            btnAdmin = new System.Windows.Forms.Button();
            btnLogout = new System.Windows.Forms.Button();
            btnZoomIn = new System.Windows.Forms.Button();
            btnZoomOut = new System.Windows.Forms.Button();

            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();

            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();

            // ================= SPLIT =================
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.SplitterDistance = 550;

            // ================= LEFT PANEL =================
            splitContainer1.Panel1.Controls.Add(txtScan);
            splitContainer1.Panel1.Controls.Add(cmbPart);
            splitContainer1.Panel1.Controls.Add(lblStatus);
            splitContainer1.Panel1.Controls.Add(lblTotal);
            splitContainer1.Panel1.Controls.Add(lblToday);
            splitContainer1.Panel1.Controls.Add(lblDateTime);
            splitContainer1.Panel1.Controls.Add(lblShift);
            splitContainer1.Panel1.Controls.Add(lblUser);
            splitContainer1.Panel1.Controls.Add(lstStatus);
            splitContainer1.Panel1.Controls.Add(btnClear);
            splitContainer1.Panel1.Controls.Add(btnReset);
            splitContainer1.Panel1.Controls.Add(btnOpenCsv);
            splitContainer1.Panel1.Controls.Add(btnTestMail);
            splitContainer1.Panel1.Controls.Add(btnAdmin);
            splitContainer1.Panel1.Controls.Add(btnLogout);
            splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);

            // ================= RIGHT PANEL =================
            splitContainer1.Panel2.Controls.Add(panelPdf);
            splitContainer1.Panel2.Controls.Add(panelTop);

            // ================= TOP PANEL (ZOOM BAR) =================
            panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            panelTop.Height = 40;
            panelTop.BackColor = System.Drawing.Color.FromArgb(230, 230, 230);
            panelTop.Controls.Add(btnZoomIn);
            panelTop.Controls.Add(btnZoomOut);

            btnZoomIn.Text = "🔍+";
            btnZoomIn.Location = new System.Drawing.Point(10, 7);
            btnZoomIn.Size = new System.Drawing.Size(55, 26);
            btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnZoomIn.Click += btnZoomIn_Click;

            btnZoomOut.Text = "🔍-";
            btnZoomOut.Location = new System.Drawing.Point(75, 7);
            btnZoomOut.Size = new System.Drawing.Size(55, 26);
            btnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnZoomOut.Click += btnZoomOut_Click;

            // ================= PDF PANEL =================
            panelPdf.Dock = System.Windows.Forms.DockStyle.Fill;
            panelPdf.Controls.Add(webView21);
            webView21.Dock = System.Windows.Forms.DockStyle.Fill;

            // ================= SCAN INPUT =================
            txtScan.Location = new System.Drawing.Point(20, 20);
            txtScan.Size = new System.Drawing.Size(280, 27);
            txtScan.Font = new System.Drawing.Font("Segoe UI", 11);
            txtScan.PlaceholderText = "Scan barcode here...";
            txtScan.KeyDown += txtScan_KeyDown;

            cmbPart.Location = new System.Drawing.Point(315, 20);
            cmbPart.Size = new System.Drawing.Size(180, 28);
            cmbPart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPart.SelectedIndexChanged += cmbPart_SelectedIndexChanged;

            // ================= STATUS LABEL =================
            lblStatus.Location = new System.Drawing.Point(20, 65);
            lblStatus.Size = new System.Drawing.Size(200, 35);
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblStatus.ForeColor = System.Drawing.Color.Green;
            lblStatus.Text = "READY";

            // ================= COUNTERS =================
            lblTotal.Location = new System.Drawing.Point(20, 110);
            lblTotal.AutoSize = true;
            lblTotal.Font = new System.Drawing.Font("Segoe UI", 10);

            lblToday.Location = new System.Drawing.Point(20, 135);
            lblToday.AutoSize = true;
            lblToday.Font = new System.Drawing.Font("Segoe UI", 10);

            // ================= INFO LABELS =================
            lblDateTime.Location = new System.Drawing.Point(20, 165);
            lblDateTime.AutoSize = true;
            lblDateTime.Font = new System.Drawing.Font("Segoe UI", 9);
            lblDateTime.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);

            lblShift.Location = new System.Drawing.Point(20, 188);
            lblShift.AutoSize = true;
            lblShift.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            lblShift.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);
            lblShift.Text = "Shift: -";

            lblUser.Location = new System.Drawing.Point(20, 211);
            lblUser.AutoSize = true;
            lblUser.Font = new System.Drawing.Font("Segoe UI", 9);
            lblUser.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            lblUser.Text = "User: -";

            // ================= LOG LIST =================
            lstStatus.Location = new System.Drawing.Point(20, 245);
            lstStatus.Size = new System.Drawing.Size(500, 220);
            lstStatus.Font = new System.Drawing.Font("Consolas", 8);
            lstStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ================= BUTTONS =================
            int btnTop = 480;

            btnClear.Location = new System.Drawing.Point(20, btnTop);
            btnClear.Size = new System.Drawing.Size(80, 30);
            btnClear.Text = "Clear";
            btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClear.Click += btnClear_Click;

            btnReset.Location = new System.Drawing.Point(110, btnTop);
            btnReset.Size = new System.Drawing.Size(80, 30);
            btnReset.Text = "Reset";
            btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnReset.Click += btnReset_Click;

            btnOpenCsv.Location = new System.Drawing.Point(200, btnTop);
            btnOpenCsv.Size = new System.Drawing.Size(100, 30);
            btnOpenCsv.Text = "Open CSV";
            btnOpenCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnOpenCsv.Click += btnOpenCsv_Click;

            btnTestMail.Location = new System.Drawing.Point(310, btnTop);
            btnTestMail.Size = new System.Drawing.Size(100, 30);
            btnTestMail.Text = "Send Mail";
            btnTestMail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnTestMail.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            btnTestMail.ForeColor = System.Drawing.Color.White;
            btnTestMail.Click += btnTestMail_Click;

            btnAdmin.Location = new System.Drawing.Point(20, btnTop + 40);
            btnAdmin.Size = new System.Drawing.Size(120, 30);
            btnAdmin.Text = "⚙ Admin Panel";
            btnAdmin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAdmin.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            btnAdmin.ForeColor = System.Drawing.Color.White;
            btnAdmin.Click += btnAdmin_Click;

            btnLogout.Location = new System.Drawing.Point(155, btnTop + 40);
            btnLogout.Size = new System.Drawing.Size(100, 30);
            btnLogout.Text = "🚪 Logout";
            btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnLogout.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            btnLogout.ForeColor = System.Drawing.Color.White;
            btnLogout.Click += btnLogout_Click;

            // ================= FORM =================
            ClientSize = new System.Drawing.Size(1400, 750);
            Controls.Add(splitContainer1);
            Text = "📦 Packaging EOL System";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += Form1_Load;

            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}