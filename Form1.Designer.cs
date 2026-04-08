namespace BarcodeBartenderApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            splitContainerMain = new SplitContainer();
            splitContainerLeft = new SplitContainer();
            panelTopBar = new Panel();
            lblDateTime = new Label();
            lblUser = new Label();
            lblShift = new Label();
            btnAdmin = new Button();
            btnLogout = new Button();
            panelStats = new Panel();
            lblTotal = new Label();
            lblToday = new Label();
            progressShift = new ProgressBar();
            lblProgress = new Label();
            panelScan = new Panel();
            lblScanTitle = new Label();
            txtScan = new TextBox();
            lblInspectorTitle = new Label();
            txtInspector = new TextBox();
            lblStatus = new Label();
            panelActiveOrder = new Panel();
            lblActiveOrderTitle = new Label();
            lblActiveOrder = new Label();
            lblActiveCustomer = new Label();
            lblActivePart = new Label();
            lblActiveQty = new Label();
            progressDispatch = new ProgressBar();
            btnCancelOrder = new Button();
            panelTokenTitle = new Panel();
            lblTokenTitle = new Label();
            btnRefreshTokens = new Button();
            flpTokens = new FlowLayoutPanel();
            splitContainerRight = new SplitContainer();
            panelPartSop = new Panel();
            lblPartLabel = new Label();
            cmbPart = new ComboBox();
            btnZoomIn = new Button();
            btnZoomOut = new Button();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            panelLog = new Panel();
            lblLogTitle = new Label();
            lstStatus = new ListBox();
            btnClear = new Button();
            btnReset = new Button();
            btnOpenCsv = new Button();
            btnTestMail = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).BeginInit();
            splitContainerLeft.Panel1.SuspendLayout();
            splitContainerLeft.Panel2.SuspendLayout();
            splitContainerLeft.SuspendLayout();
            panelTopBar.SuspendLayout();
            panelStats.SuspendLayout();
            panelScan.SuspendLayout();
            panelActiveOrder.SuspendLayout();
            panelTokenTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).BeginInit();
            splitContainerRight.Panel1.SuspendLayout();
            splitContainerRight.Panel2.SuspendLayout();
            splitContainerRight.SuspendLayout();
            panelPartSop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            panelLog.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(0, 0);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(splitContainerLeft);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(splitContainerRight);
            splitContainerMain.Size = new Size(1400, 860);
            splitContainerMain.SplitterDistance = 760;
            splitContainerMain.TabIndex = 0;
            // 
            // splitContainerLeft
            // 
            splitContainerLeft.Dock = DockStyle.Fill;
            splitContainerLeft.Location = new Point(0, 0);
            splitContainerLeft.Name = "splitContainerLeft";
            splitContainerLeft.Orientation = Orientation.Horizontal;
            // 
            // splitContainerLeft.Panel1
            // 
            splitContainerLeft.Panel1.Controls.Add(panelTopBar);
            splitContainerLeft.Panel1.Controls.Add(panelStats);
            splitContainerLeft.Panel1.Controls.Add(panelScan);
            splitContainerLeft.Panel1.Controls.Add(panelActiveOrder);
            // 
            // splitContainerLeft.Panel2
            // 
            splitContainerLeft.Panel2.Controls.Add(panelTokenTitle);
            splitContainerLeft.Panel2.Controls.Add(flpTokens);
            splitContainerLeft.Size = new Size(760, 860);
            splitContainerLeft.SplitterDistance = 460;
            splitContainerLeft.TabIndex = 0;
            // 
            // panelTopBar
            // 
            panelTopBar.BackColor = Color.FromArgb(24, 48, 96);
            panelTopBar.Controls.Add(lblDateTime);
            panelTopBar.Controls.Add(lblUser);
            panelTopBar.Controls.Add(lblShift);
            panelTopBar.Controls.Add(btnAdmin);
            panelTopBar.Controls.Add(btnLogout);
            panelTopBar.Dock = DockStyle.Top;
            panelTopBar.Location = new Point(0, 150);
            panelTopBar.Name = "panelTopBar";
            panelTopBar.Size = new Size(760, 54);
            panelTopBar.TabIndex = 0;
            // 
            // lblDateTime
            // 
            lblDateTime.AutoSize = true;
            lblDateTime.Font = new Font("Segoe UI", 9.5F);
            lblDateTime.ForeColor = Color.White;
            lblDateTime.Location = new Point(10, 17);
            lblDateTime.Name = "lblDateTime";
            lblDateTime.Size = new Size(158, 21);
            lblDateTime.TabIndex = 0;
            lblDateTime.Text = "00-00-0000 00:00:00";
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9.5F);
            lblUser.ForeColor = Color.FromArgb(180, 210, 255);
            lblUser.Location = new Point(185, 17);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(65, 21);
            lblUser.TabIndex = 1;
            lblUser.Text = "User: —";
            // 
            // lblShift
            // 
            lblShift.AutoSize = true;
            lblShift.Font = new Font("Segoe UI", 9.5F);
            lblShift.ForeColor = Color.FromArgb(180, 210, 255);
            lblShift.Location = new Point(310, 17);
            lblShift.Name = "lblShift";
            lblShift.Size = new Size(65, 21);
            lblShift.TabIndex = 2;
            lblShift.Text = "Shift: —";
            // 
            // btnAdmin
            // 
            btnAdmin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdmin.BackColor = Color.Transparent;
            btnAdmin.FlatAppearance.BorderColor = Color.FromArgb(180, 210, 255);
            btnAdmin.FlatStyle = FlatStyle.Flat;
            btnAdmin.Font = new Font("Segoe UI", 8.5F);
            btnAdmin.ForeColor = Color.White;
            btnAdmin.Location = new Point(566, 12);
            btnAdmin.Name = "btnAdmin";
            btnAdmin.Size = new Size(80, 30);
            btnAdmin.TabIndex = 3;
            btnAdmin.Text = "Admin";
            btnAdmin.UseVisualStyleBackColor = false;
            btnAdmin.Click += btnAdmin_Click;
            // 
            // btnLogout
            // 
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLogout.BackColor = Color.Transparent;
            btnLogout.FlatAppearance.BorderColor = Color.FromArgb(180, 210, 255);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Font = new Font("Segoe UI", 8.5F);
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(654, 12);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(80, 30);
            btnLogout.TabIndex = 4;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // panelStats
            // 
            panelStats.BackColor = Color.FromArgb(235, 240, 250);
            panelStats.Controls.Add(lblTotal);
            panelStats.Controls.Add(lblToday);
            panelStats.Controls.Add(progressShift);
            panelStats.Controls.Add(lblProgress);
            panelStats.Dock = DockStyle.Top;
            panelStats.Location = new Point(0, 90);
            panelStats.Name = "panelStats";
            panelStats.Padding = new Padding(8, 8, 8, 4);
            panelStats.Size = new Size(760, 60);
            panelStats.TabIndex = 1;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(30, 30, 80);
            lblTotal.Location = new Point(10, 20);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(61, 20);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total: 0";
            // 
            // lblToday
            // 
            lblToday.AutoSize = true;
            lblToday.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblToday.ForeColor = Color.FromArgb(30, 30, 80);
            lblToday.Location = new Point(110, 20);
            lblToday.Name = "lblToday";
            lblToday.Size = new Size(68, 20);
            lblToday.TabIndex = 1;
            lblToday.Text = "Today: 0";
            // 
            // progressShift
            // 
            progressShift.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressShift.Location = new Point(210, 22);
            progressShift.Name = "progressShift";
            progressShift.Size = new Size(220, 14);
            progressShift.TabIndex = 2;
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Font = new Font("Segoe UI", 8.5F);
            lblProgress.ForeColor = Color.FromArgb(60, 60, 100);
            lblProgress.Location = new Point(445, 20);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(155, 20);
            lblProgress.TabIndex = 3;
            lblProgress.Text = "Shift: 0 (No target set)";
            // 
            // panelScan
            // 
            panelScan.BackColor = Color.White;
            panelScan.Controls.Add(lblScanTitle);
            panelScan.Controls.Add(txtScan);
            panelScan.Controls.Add(lblInspectorTitle);
            panelScan.Controls.Add(txtInspector);
            panelScan.Controls.Add(lblStatus);
            panelScan.Dock = DockStyle.Top;
            panelScan.Location = new Point(0, 0);
            panelScan.Name = "panelScan";
            panelScan.Size = new Size(760, 148);
            panelScan.TabIndex = 2;
            //
            // lblScanTitle
            //
            lblScanTitle.AutoSize = true;
            lblScanTitle.Font = new Font("Segoe UI", 8.5F);
            lblScanTitle.ForeColor = Color.FromArgb(100, 100, 120);
            lblScanTitle.Location = new Point(10, 6);
            lblScanTitle.Name = "lblScanTitle";
            lblScanTitle.Size = new Size(102, 20);
            lblScanTitle.TabIndex = 0;
            lblScanTitle.Text = "Scan barcode:";
            //
            // txtScan
            //
            txtScan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtScan.BackColor = Color.FromArgb(240, 255, 240);
            txtScan.Font = new Font("Segoe UI", 13F);
            txtScan.Location = new Point(10, 24);
            txtScan.MaxLength = 200;
            txtScan.Name = "txtScan";
            txtScan.PlaceholderText = "Scan or type barcode here...";
            txtScan.Size = new Size(580, 36);
            txtScan.TabIndex = 1;
            txtScan.KeyDown += txtScan_KeyDown;
            txtScan.KeyPress += txtScan_KeyPress;
            //
            // lblInspectorTitle
            //
            lblInspectorTitle.AutoSize = true;
            lblInspectorTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblInspectorTitle.ForeColor = Color.FromArgb(160, 60, 0);
            lblInspectorTitle.Location = new Point(10, 68);
            lblInspectorTitle.Name = "lblInspectorTitle";
            lblInspectorTitle.Size = new Size(130, 20);
            lblInspectorTitle.TabIndex = 2;
            lblInspectorTitle.Text = "Inspector Name *:";
            //
            // txtInspector
            //
            txtInspector.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtInspector.BackColor = Color.FromArgb(255, 250, 235);
            txtInspector.Font = new Font("Segoe UI", 10.5F);
            txtInspector.Location = new Point(10, 86);
            txtInspector.MaxLength = 80;
            txtInspector.Name = "txtInspector";
            txtInspector.PlaceholderText = "Enter inspector name before scanning...";
            txtInspector.Size = new Size(580, 30);
            txtInspector.TabIndex = 3;
            //
            // lblStatus
            //
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(0, 120, 215);
            lblStatus.Location = new Point(10, 124);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(58, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "READY";
            // 
            // panelActiveOrder
            // 
            panelActiveOrder.BackColor = Color.FromArgb(248, 250, 255);
            panelActiveOrder.Controls.Add(lblActiveOrderTitle);
            panelActiveOrder.Controls.Add(lblActiveOrder);
            panelActiveOrder.Controls.Add(lblActiveCustomer);
            panelActiveOrder.Controls.Add(lblActivePart);
            panelActiveOrder.Controls.Add(lblActiveQty);
            panelActiveOrder.Controls.Add(progressDispatch);
            panelActiveOrder.Controls.Add(btnCancelOrder);
            panelActiveOrder.Dock = DockStyle.Fill;
            panelActiveOrder.Location = new Point(0, 0);
            panelActiveOrder.Name = "panelActiveOrder";
            panelActiveOrder.Padding = new Padding(10);
            panelActiveOrder.Size = new Size(760, 460);
            panelActiveOrder.TabIndex = 3;
            // 
            // lblActiveOrderTitle
            // 
            lblActiveOrderTitle.AutoSize = true;
            lblActiveOrderTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblActiveOrderTitle.ForeColor = Color.FromArgb(80, 80, 120);
            lblActiveOrderTitle.Location = new Point(10, 8);
            lblActiveOrderTitle.Name = "lblActiveOrderTitle";
            lblActiveOrderTitle.Size = new Size(115, 20);
            lblActiveOrderTitle.TabIndex = 0;
            lblActiveOrderTitle.Text = "ACTIVE ORDER";
            // 
            // lblActiveOrder
            // 
            lblActiveOrder.AutoSize = true;
            lblActiveOrder.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblActiveOrder.ForeColor = Color.FromArgb(20, 60, 140);
            lblActiveOrder.Location = new Point(10, 30);
            lblActiveOrder.Name = "lblActiveOrder";
            lblActiveOrder.Size = new Size(170, 25);
            lblActiveOrder.TabIndex = 1;
            lblActiveOrder.Text = "No order selected";
            // 
            // lblActiveCustomer
            // 
            lblActiveCustomer.AutoSize = true;
            lblActiveCustomer.Font = new Font("Segoe UI", 9F);
            lblActiveCustomer.ForeColor = Color.FromArgb(60, 60, 80);
            lblActiveCustomer.Location = new Point(10, 58);
            lblActiveCustomer.Name = "lblActiveCustomer";
            lblActiveCustomer.Size = new Size(0, 20);
            lblActiveCustomer.TabIndex = 2;
            // 
            // lblActivePart
            // 
            lblActivePart.AutoSize = true;
            lblActivePart.Font = new Font("Segoe UI", 9F);
            lblActivePart.ForeColor = Color.FromArgb(80, 80, 100);
            lblActivePart.Location = new Point(10, 80);
            lblActivePart.Name = "lblActivePart";
            lblActivePart.Size = new Size(0, 20);
            lblActivePart.TabIndex = 3;
            // 
            // lblActiveQty
            // 
            lblActiveQty.AutoSize = true;
            lblActiveQty.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblActiveQty.ForeColor = Color.FromArgb(0, 120, 60);
            lblActiveQty.Location = new Point(10, 102);
            lblActiveQty.Name = "lblActiveQty";
            lblActiveQty.Size = new Size(0, 23);
            lblActiveQty.TabIndex = 4;
            // 
            // progressDispatch
            // 
            progressDispatch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressDispatch.Location = new Point(10, 130);
            progressDispatch.Name = "progressDispatch";
            progressDispatch.Size = new Size(730, 18);
            progressDispatch.Style = ProgressBarStyle.Continuous;
            progressDispatch.TabIndex = 5;
            // 
            // btnCancelOrder
            // 
            btnCancelOrder.BackColor = Color.FromArgb(255, 240, 240);
            btnCancelOrder.FlatAppearance.BorderColor = Color.FromArgb(200, 100, 100);
            btnCancelOrder.FlatStyle = FlatStyle.Flat;
            btnCancelOrder.Font = new Font("Segoe UI", 8.5F);
            btnCancelOrder.ForeColor = Color.FromArgb(180, 30, 30);
            btnCancelOrder.Location = new Point(10, 158);
            btnCancelOrder.Name = "btnCancelOrder";
            btnCancelOrder.Size = new Size(140, 30);
            btnCancelOrder.TabIndex = 6;
            btnCancelOrder.Text = "Cancel / Release";
            btnCancelOrder.UseVisualStyleBackColor = false;
            btnCancelOrder.Click += btnCancelOrder_Click;
            // 
            // panelTokenTitle
            // 
            panelTokenTitle.BackColor = Color.FromArgb(235, 240, 250);
            panelTokenTitle.Controls.Add(lblTokenTitle);
            panelTokenTitle.Controls.Add(btnRefreshTokens);
            panelTokenTitle.Dock = DockStyle.Top;
            panelTokenTitle.Location = new Point(0, 0);
            panelTokenTitle.Name = "panelTokenTitle";
            panelTokenTitle.Size = new Size(760, 36);
            panelTokenTitle.TabIndex = 0;
            // 
            // lblTokenTitle
            // 
            lblTokenTitle.AutoSize = true;
            lblTokenTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblTokenTitle.ForeColor = Color.FromArgb(30, 30, 80);
            lblTokenTitle.Location = new Point(10, 9);
            lblTokenTitle.Name = "lblTokenTitle";
            lblTokenTitle.Size = new Size(134, 21);
            lblTokenTitle.TabIndex = 0;
            lblTokenTitle.Text = "Dispatch Tokens";
            // 
            // btnRefreshTokens
            // 
            btnRefreshTokens.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshTokens.BackColor = Color.FromArgb(220, 230, 245);
            btnRefreshTokens.FlatStyle = FlatStyle.Flat;
            btnRefreshTokens.Font = new Font("Segoe UI", 8F);
            btnRefreshTokens.ForeColor = Color.FromArgb(30, 60, 120);
            btnRefreshTokens.Location = new Point(672, 6);
            btnRefreshTokens.Name = "btnRefreshTokens";
            btnRefreshTokens.Size = new Size(80, 24);
            btnRefreshTokens.TabIndex = 1;
            btnRefreshTokens.Text = "Refresh";
            btnRefreshTokens.UseVisualStyleBackColor = false;
            btnRefreshTokens.Click += btnRefreshTokens_Click;
            // 
            // flpTokens
            // 
            flpTokens.AutoScroll = true;
            flpTokens.BackColor = Color.FromArgb(245, 246, 250);
            flpTokens.Dock = DockStyle.Fill;
            flpTokens.FlowDirection = FlowDirection.LeftToRight;
            flpTokens.WrapContents = true;
            flpTokens.Location = new Point(0, 0);
            flpTokens.Name = "flpTokens";
            flpTokens.Padding = new Padding(6);
            flpTokens.Size = new Size(760, 396);
            flpTokens.TabIndex = 1;
            // 
            // splitContainerRight
            // 
            splitContainerRight.Dock = DockStyle.Fill;
            splitContainerRight.Location = new Point(0, 0);
            splitContainerRight.Name = "splitContainerRight";
            splitContainerRight.Orientation = Orientation.Horizontal;
            // 
            // splitContainerRight.Panel1
            // 
            splitContainerRight.Panel1.Controls.Add(panelPartSop);
            // 
            // splitContainerRight.Panel2
            // 
            splitContainerRight.Panel2.Controls.Add(panelLog);
            splitContainerRight.Size = new Size(636, 860);
            splitContainerRight.SplitterDistance = 610;
            splitContainerRight.TabIndex = 0;
            // 
            // panelPartSop
            // 
            panelPartSop.BackColor = Color.White;
            panelPartSop.Controls.Add(lblPartLabel);
            panelPartSop.Controls.Add(cmbPart);
            panelPartSop.Controls.Add(btnZoomIn);
            panelPartSop.Controls.Add(btnZoomOut);
            panelPartSop.Controls.Add(webView21);
            panelPartSop.Dock = DockStyle.Fill;
            panelPartSop.Location = new Point(0, 0);
            panelPartSop.Name = "panelPartSop";
            panelPartSop.Size = new Size(636, 610);
            panelPartSop.TabIndex = 0;
            // 
            // lblPartLabel
            // 
            lblPartLabel.AutoSize = true;
            lblPartLabel.Font = new Font("Segoe UI", 8.5F);
            lblPartLabel.ForeColor = Color.FromArgb(80, 80, 100);
            lblPartLabel.Location = new Point(8, 10);
            lblPartLabel.Name = "lblPartLabel";
            lblPartLabel.Size = new Size(78, 20);
            lblPartLabel.TabIndex = 0;
            lblPartLabel.Text = "Part / SOP:";
            // 
            // cmbPart
            // 
            cmbPart.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbPart.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPart.Font = new Font("Segoe UI", 9F);
            cmbPart.Location = new Point(90, 6);
            cmbPart.Name = "cmbPart";
            cmbPart.Size = new Size(400, 28);
            cmbPart.TabIndex = 1;
            cmbPart.SelectedIndexChanged += cmbPart_SelectedIndexChanged;
            // 
            // btnZoomIn
            // 
            btnZoomIn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnZoomIn.FlatStyle = FlatStyle.Flat;
            btnZoomIn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnZoomIn.Location = new Point(500, 6);
            btnZoomIn.Name = "btnZoomIn";
            btnZoomIn.Size = new Size(32, 28);
            btnZoomIn.TabIndex = 2;
            btnZoomIn.Text = "+";
            btnZoomIn.Click += btnZoomIn_Click;
            // 
            // btnZoomOut
            // 
            btnZoomOut.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnZoomOut.FlatStyle = FlatStyle.Flat;
            btnZoomOut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnZoomOut.Location = new Point(538, 6);
            btnZoomOut.Name = "btnZoomOut";
            btnZoomOut.Size = new Size(32, 28);
            btnZoomOut.TabIndex = 3;
            btnZoomOut.Text = "−";
            btnZoomOut.Click += btnZoomOut_Click;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(0, 40);
            webView21.Name = "webView21";
            webView21.Size = new Size(636, 570);
            webView21.TabIndex = 4;
            webView21.ZoomFactor = 1D;
            // 
            // panelLog
            // 
            panelLog.BackColor = Color.White;
            panelLog.Controls.Add(lblLogTitle);
            panelLog.Controls.Add(lstStatus);
            panelLog.Controls.Add(btnClear);
            panelLog.Controls.Add(btnReset);
            panelLog.Controls.Add(btnOpenCsv);
            panelLog.Controls.Add(btnTestMail);
            panelLog.Dock = DockStyle.Fill;
            panelLog.Location = new Point(0, 0);
            panelLog.Name = "panelLog";
            panelLog.Padding = new Padding(6);
            panelLog.Size = new Size(636, 246);
            panelLog.TabIndex = 0;
            // 
            // lblLogTitle
            // 
            lblLogTitle.AutoSize = true;
            lblLogTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblLogTitle.ForeColor = Color.FromArgb(80, 80, 100);
            lblLogTitle.Location = new Point(6, 6);
            lblLogTitle.Name = "lblLogTitle";
            lblLogTitle.Size = new Size(71, 20);
            lblLogTitle.TabIndex = 0;
            lblLogTitle.Text = "Scan Log";
            // 
            // lstStatus
            // 
            lstStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstStatus.BorderStyle = BorderStyle.FixedSingle;
            lstStatus.Font = new Font("Consolas", 8.5F);
            lstStatus.ItemHeight = 17;
            lstStatus.Location = new Point(6, 26);
            lstStatus.Name = "lstStatus";
            lstStatus.Size = new Size(624, 172);
            lstStatus.TabIndex = 1;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Segoe UI", 8.5F);
            btnClear.Location = new Point(6, 210);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(90, 28);
            btnClear.TabIndex = 2;
            btnClear.Text = "Clear Log";
            btnClear.Click += btnClear_Click;
            // 
            // btnReset
            // 
            btnReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Font = new Font("Segoe UI", 8.5F);
            btnReset.Location = new Point(102, 210);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(100, 28);
            btnReset.TabIndex = 3;
            btnReset.Text = "Reset Count";
            btnReset.Click += btnReset_Click;
            // 
            // btnOpenCsv
            // 
            btnOpenCsv.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenCsv.FlatStyle = FlatStyle.Flat;
            btnOpenCsv.Font = new Font("Segoe UI", 8.5F);
            btnOpenCsv.Location = new Point(208, 210);
            btnOpenCsv.Name = "btnOpenCsv";
            btnOpenCsv.Size = new Size(90, 28);
            btnOpenCsv.TabIndex = 4;
            btnOpenCsv.Text = "Open CSV";
            btnOpenCsv.Click += btnOpenCsv_Click;
            // 
            // btnTestMail
            // 
            btnTestMail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnTestMail.FlatStyle = FlatStyle.Flat;
            btnTestMail.Font = new Font("Segoe UI", 8.5F);
            btnTestMail.Location = new Point(304, 210);
            btnTestMail.Name = "btnTestMail";
            btnTestMail.Size = new Size(90, 28);
            btnTestMail.TabIndex = 5;
            btnTestMail.Text = "Test Mail";
            btnTestMail.Click += btnTestMail_Click;
            // 
            // Form1
            // 
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1400, 860);
            Controls.Add(splitContainerMain);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1100, 700);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dispatch System";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            splitContainerLeft.Panel1.ResumeLayout(false);
            splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).EndInit();
            splitContainerLeft.ResumeLayout(false);
            panelTopBar.ResumeLayout(false);
            panelTopBar.PerformLayout();
            panelStats.ResumeLayout(false);
            panelStats.PerformLayout();
            panelScan.ResumeLayout(false);
            panelScan.PerformLayout();
            panelActiveOrder.ResumeLayout(false);
            panelActiveOrder.PerformLayout();
            panelTokenTitle.ResumeLayout(false);
            panelTokenTitle.PerformLayout();
            splitContainerRight.Panel1.ResumeLayout(false);
            splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).EndInit();
            splitContainerRight.ResumeLayout(false);
            panelPartSop.ResumeLayout(false);
            panelPartSop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            panelLog.ResumeLayout(false);
            panelLog.PerformLayout();
            ResumeLayout(false);
        }

        // ── control declarations ─────────────────────────────────────────
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private System.Windows.Forms.Panel panelTopBar;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblShift;
        private System.Windows.Forms.Button btnAdmin;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelStats;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblToday;
        private System.Windows.Forms.ProgressBar progressShift;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Panel panelScan;
        private System.Windows.Forms.Label lblScanTitle;
        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.Label lblInspectorTitle;
        private System.Windows.Forms.TextBox txtInspector;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelActiveOrder;
        private System.Windows.Forms.Label lblActiveOrderTitle;
        private System.Windows.Forms.Label lblActiveOrder;
        private System.Windows.Forms.Label lblActiveCustomer;
        private System.Windows.Forms.Label lblActivePart;
        private System.Windows.Forms.Label lblActiveQty;
        private System.Windows.Forms.ProgressBar progressDispatch;
        private System.Windows.Forms.Button btnCancelOrder;
        private System.Windows.Forms.Panel panelTokenTitle;
        private System.Windows.Forms.Label lblTokenTitle;
        private System.Windows.Forms.Button btnRefreshTokens;
        private System.Windows.Forms.FlowLayoutPanel flpTokens;
        private System.Windows.Forms.Panel panelPartSop;
        private System.Windows.Forms.Label lblPartLabel;
        private System.Windows.Forms.ComboBox cmbPart;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Panel panelLog;
        private System.Windows.Forms.Label lblLogTitle;
        private System.Windows.Forms.ListBox lstStatus;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnOpenCsv;
        private System.Windows.Forms.Button btnTestMail;
    }
}