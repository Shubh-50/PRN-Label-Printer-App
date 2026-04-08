namespace BarcodeBartenderApp
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();

            // ── Tab pages ────────────────────────────────────────────────
            this.tabUsers = new System.Windows.Forms.TabPage();
            this.tabEmail = new System.Windows.Forms.TabPage();
            this.tabShift = new System.Windows.Forms.TabPage();
            this.tabParts = new System.Windows.Forms.TabPage();
            this.tabPrn = new System.Windows.Forms.TabPage();
            this.tabDispatch = new System.Windows.Forms.TabPage();
            this.tabCustomerPrn = new System.Windows.Forms.TabPage();
            this.tabPrinter = new System.Windows.Forms.TabPage();

            // ── Users tab controls ───────────────────────────────────────
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblUserPass = new System.Windows.Forms.Label();

            // ── Email tab controls ───────────────────────────────────────
            this.lblSender = new System.Windows.Forms.Label();
            this.txtSender = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblReceiver = new System.Windows.Forms.Label();
            this.txtReceiver = new System.Windows.Forms.TextBox();
            this.btnSaveEmail = new System.Windows.Forms.Button();
            this.btnTestMail = new System.Windows.Forms.Button();
            this.btnOpenCsv = new System.Windows.Forms.Button();

            // ── Shift tab controls ───────────────────────────────────────
            this.lblShiftA = new System.Windows.Forms.Label();
            this.txtAStart = new System.Windows.Forms.TextBox();
            this.txtAEnd = new System.Windows.Forms.TextBox();
            this.txtTargetA = new System.Windows.Forms.TextBox();
            this.lblShiftB = new System.Windows.Forms.Label();
            this.txtBStart = new System.Windows.Forms.TextBox();
            this.txtBEnd = new System.Windows.Forms.TextBox();
            this.txtTargetB = new System.Windows.Forms.TextBox();
            this.lblShiftC = new System.Windows.Forms.Label();
            this.txtCStart = new System.Windows.Forms.TextBox();
            this.txtCEnd = new System.Windows.Forms.TextBox();
            this.txtTargetC = new System.Windows.Forms.TextBox();
            this.btnSaveShift = new System.Windows.Forms.Button();

            // ── Parts / SOP tab controls ─────────────────────────────────
            this.lstParts = new System.Windows.Forms.ListBox();
            this.txtNewPart = new System.Windows.Forms.TextBox();
            this.btnAddPart = new System.Windows.Forms.Button();
            this.btnDeletePart = new System.Windows.Forms.Button();
            this.cmbPartSop = new System.Windows.Forms.ComboBox();
            this.btnUploadPdf = new System.Windows.Forms.Button();
            this.lblPdfName = new System.Windows.Forms.Label();

            // ── PRN Editor tab controls ──────────────────────────────────
            this.cmbPrnPart = new System.Windows.Forms.ComboBox();
            this.txtPrnEditor = new System.Windows.Forms.TextBox();
            this.txtPrnPath = new System.Windows.Forms.TextBox();
            this.btnPrnLoad = new System.Windows.Forms.Button();
            this.btnPrnSave = new System.Windows.Forms.Button();

            // ── Dispatch Orders tab controls ─────────────────────────────
            this.lstDispatchOrders = new System.Windows.Forms.ListView();
            this.colOrderNo = new System.Windows.Forms.ColumnHeader();
            this.colCustomer = new System.Windows.Forms.ColumnHeader();
            this.colPart = new System.Windows.Forms.ColumnHeader();
            this.colQty = new System.Windows.Forms.ColumnHeader();
            this.colScanned = new System.Windows.Forms.ColumnHeader();
            this.colStatus = new System.Windows.Forms.ColumnHeader();
            this.colDue = new System.Windows.Forms.ColumnHeader();
            this.lblOrderCustomer = new System.Windows.Forms.Label();
            this.txtOrderCustomer = new System.Windows.Forms.TextBox();
            this.lblOrderPart = new System.Windows.Forms.Label();
            this.txtOrderPart = new System.Windows.Forms.TextBox();
            this.lblOrderQRRef = new System.Windows.Forms.Label();
            this.txtOrderQRRef = new System.Windows.Forms.TextBox();
            this.lblOrderQty = new System.Windows.Forms.Label();
            this.txtOrderQty = new System.Windows.Forms.TextBox();
            this.lblOrderDue = new System.Windows.Forms.Label();
            this.dtpOrderDue = new System.Windows.Forms.DateTimePicker();
            this.btnCreateOrder = new System.Windows.Forms.Button();
            this.btnDeleteOrder = new System.Windows.Forms.Button();
            this.btnRefreshOrders = new System.Windows.Forms.Button();
            this.btnEditOrder = new System.Windows.Forms.Button();
            this.btnNewOrder = new System.Windows.Forms.Button();

            // ── Customer PRN tab controls ────────────────────────────────
            this.cmbCustomerPrn = new System.Windows.Forms.ComboBox();
            this.txtCustomerPrnEditor = new System.Windows.Forms.TextBox();
            this.txtCustomerPrnPath = new System.Windows.Forms.TextBox();
            this.btnCustomerPrnLoad = new System.Windows.Forms.Button();
            this.btnCustomerPrnSave = new System.Windows.Forms.Button();
            this.btnRefreshCustomerPrn = new System.Windows.Forms.Button();

            // ── Printer tab controls ─────────────────────────────────────
            this.lblPrinterName = new System.Windows.Forms.Label();
            this.txtPrinterName = new System.Windows.Forms.TextBox();
            this.btnSavePrinter = new System.Windows.Forms.Button();

            this.tabControl.SuspendLayout();
            this.SuspendLayout();

            // ── Form ─────────────────────────────────────────────────────
            this.Text = "Admin Panel";
            this.ClientSize = new System.Drawing.Size(900, 680);
            this.MinimumSize = new System.Drawing.Size(900, 680);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BackColor = System.Drawing.Color.FromArgb(245, 246, 250);
            this.Controls.Add(this.tabControl);

            // ── tabControl ───────────────────────────────────────────────
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControl.TabPages.AddRange(new System.Windows.Forms.TabPage[] {
                this.tabUsers, this.tabEmail, this.tabShift,
                this.tabParts,
                this.tabDispatch, this.tabCustomerPrn, this.tabPrinter });

            // ════════════════════════════════════════════════════════════
            // TAB: USERS
            // ════════════════════════════════════════════════════════════
            this.tabUsers.Text = "Users";
            this.tabUsers.Padding = new System.Windows.Forms.Padding(10);

            this.lstUsers.Location = new System.Drawing.Point(14, 14);
            this.lstUsers.Size = new System.Drawing.Size(200, 300);
            this.lstUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstUsers.Font = new System.Drawing.Font("Segoe UI", 9.5F);

            this.lblUserName.Location = new System.Drawing.Point(230, 14);
            this.lblUserName.AutoSize = true;
            this.lblUserName.Text = "Username:";

            this.txtUser.Location = new System.Drawing.Point(230, 34);
            this.txtUser.Size = new System.Drawing.Size(200, 26);

            this.lblUserPass.Location = new System.Drawing.Point(230, 68);
            this.lblUserPass.AutoSize = true;
            this.lblUserPass.Text = "Password:";

            this.txtPass.Location = new System.Drawing.Point(230, 88);
            this.txtPass.Size = new System.Drawing.Size(200, 26);
            this.txtPass.PasswordChar = '*';

            this.btnAdd.Location = new System.Drawing.Point(230, 124);
            this.btnAdd.Size = new System.Drawing.Size(90, 30);
            this.btnAdd.Text = "Add User";
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(220, 240, 220);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            this.btnDelete.Location = new System.Drawing.Point(330, 124);
            this.btnDelete.Size = new System.Drawing.Size(100, 30);
            this.btnDelete.Text = "Delete User";
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(255, 220, 220);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.tabUsers.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lstUsers, this.lblUserName, this.txtUser,
                this.lblUserPass, this.txtPass, this.btnAdd, this.btnDelete });

            // ════════════════════════════════════════════════════════════
            // TAB: EMAIL
            // ════════════════════════════════════════════════════════════
            this.tabEmail.Text = "Email";
            this.tabEmail.Padding = new System.Windows.Forms.Padding(10);

            this.lblSender.Location = new System.Drawing.Point(14, 20);
            this.lblSender.AutoSize = true;
            this.lblSender.Text = "Sender email:";
            this.txtSender.Location = new System.Drawing.Point(14, 40);
            this.txtSender.Size = new System.Drawing.Size(300, 26);

            this.lblPassword.Location = new System.Drawing.Point(14, 74);
            this.lblPassword.AutoSize = true;
            this.lblPassword.Text = "App password:";
            this.txtPassword.Location = new System.Drawing.Point(14, 94);
            this.txtPassword.Size = new System.Drawing.Size(300, 26);
            this.txtPassword.PasswordChar = '*';

            this.lblReceiver.Location = new System.Drawing.Point(14, 128);
            this.lblReceiver.AutoSize = true;
            this.lblReceiver.Text = "Receiver email:";
            this.txtReceiver.Location = new System.Drawing.Point(14, 148);
            this.txtReceiver.Size = new System.Drawing.Size(300, 26);

            this.btnSaveEmail.Location = new System.Drawing.Point(14, 190);
            this.btnSaveEmail.Size = new System.Drawing.Size(110, 32);
            this.btnSaveEmail.Text = "Save Settings";
            this.btnSaveEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveEmail.BackColor = System.Drawing.Color.FromArgb(220, 240, 220);
            this.btnSaveEmail.Click += new System.EventHandler(this.btnSaveEmail_Click);

            this.btnTestMail.Location = new System.Drawing.Point(134, 190);
            this.btnTestMail.Size = new System.Drawing.Size(90, 32);
            this.btnTestMail.Text = "Test Mail";
            this.btnTestMail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestMail.Click += new System.EventHandler(this.btnTestMail_Click);

            this.btnOpenCsv.Location = new System.Drawing.Point(234, 190);
            this.btnOpenCsv.Size = new System.Drawing.Size(90, 32);
            this.btnOpenCsv.Text = "Open Folder";
            this.btnOpenCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenCsv.Click += new System.EventHandler(this.btnOpenCsv_Click);

            this.tabEmail.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblSender, this.txtSender, this.lblPassword, this.txtPassword,
                this.lblReceiver, this.txtReceiver, this.btnSaveEmail,
                this.btnTestMail, this.btnOpenCsv });

            // ════════════════════════════════════════════════════════════
            // TAB: SHIFT
            // ════════════════════════════════════════════════════════════
            this.tabShift.Text = "Shifts";
            this.tabShift.Padding = new System.Windows.Forms.Padding(10);

            int sy = 20;
            foreach (var (lbl, start, end, target, shift) in new[]{
                (this.lblShiftA, this.txtAStart, this.txtAEnd, this.txtTargetA, "Shift A"),
                (this.lblShiftB, this.txtBStart, this.txtBEnd, this.txtTargetB, "Shift B"),
                (this.lblShiftC, this.txtCStart, this.txtCEnd, this.txtTargetC, "Shift C")})
            {
                lbl.Location = new System.Drawing.Point(14, sy);
                lbl.AutoSize = true;
                lbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                lbl.Text = shift;
                start.Location = new System.Drawing.Point(14, sy + 22);
                start.Size = new System.Drawing.Size(70, 26);
                start.PlaceholderText = "Start";
                end.Location = new System.Drawing.Point(94, sy + 22);
                end.Size = new System.Drawing.Size(70, 26);
                end.PlaceholderText = "End";
                target.Location = new System.Drawing.Point(174, sy + 22);
                target.Size = new System.Drawing.Size(80, 26);
                target.PlaceholderText = "Target";
                sy += 70;
            }

            this.btnSaveShift.Location = new System.Drawing.Point(14, sy + 10);
            this.btnSaveShift.Size = new System.Drawing.Size(120, 32);
            this.btnSaveShift.Text = "Save Shifts";
            this.btnSaveShift.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveShift.BackColor = System.Drawing.Color.FromArgb(220, 240, 220);
            this.btnSaveShift.Click += new System.EventHandler(this.btnSaveShift_Click);

            this.tabShift.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblShiftA, this.txtAStart, this.txtAEnd, this.txtTargetA,
                this.lblShiftB, this.txtBStart, this.txtBEnd, this.txtTargetB,
                this.lblShiftC, this.txtCStart, this.txtCEnd, this.txtTargetC,
                this.btnSaveShift });

            // ════════════════════════════════════════════════════════════
            // TAB: PARTS / SOP
            // ════════════════════════════════════════════════════════════
            this.tabParts.Text = "Parts / SOP";
            this.tabParts.Padding = new System.Windows.Forms.Padding(10);

            this.lstParts.Location = new System.Drawing.Point(14, 14);
            this.lstParts.Size = new System.Drawing.Size(200, 260);
            this.lstParts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.txtNewPart.Location = new System.Drawing.Point(230, 14);
            this.txtNewPart.Size = new System.Drawing.Size(200, 26);
            this.txtNewPart.PlaceholderText = "New part name";

            this.btnAddPart.Location = new System.Drawing.Point(230, 48);
            this.btnAddPart.Size = new System.Drawing.Size(90, 30);
            this.btnAddPart.Text = "Add Part";
            this.btnAddPart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddPart.BackColor = System.Drawing.Color.FromArgb(220, 240, 220);
            this.btnAddPart.Click += new System.EventHandler(this.btnAddPart_Click);

            this.btnDeletePart.Location = new System.Drawing.Point(330, 48);
            this.btnDeletePart.Size = new System.Drawing.Size(100, 30);
            this.btnDeletePart.Text = "Delete Part";
            this.btnDeletePart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeletePart.BackColor = System.Drawing.Color.FromArgb(255, 220, 220);
            this.btnDeletePart.Click += new System.EventHandler(this.btnDeletePart_Click);

            this.cmbPartSop.Location = new System.Drawing.Point(230, 100);
            this.cmbPartSop.Size = new System.Drawing.Size(200, 26);
            this.cmbPartSop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.btnUploadPdf.Location = new System.Drawing.Point(230, 134);
            this.btnUploadPdf.Size = new System.Drawing.Size(110, 30);
            this.btnUploadPdf.Text = "Upload SOP PDF";
            this.btnUploadPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUploadPdf.Click += new System.EventHandler(this.btnUploadPdf_Click);

            this.lblPdfName.Location = new System.Drawing.Point(230, 172);
            this.lblPdfName.AutoSize = true;
            this.lblPdfName.ForeColor = System.Drawing.Color.FromArgb(60, 120, 60);
            this.lblPdfName.Text = "";

            this.tabParts.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lstParts, this.txtNewPart, this.btnAddPart, this.btnDeletePart,
                this.cmbPartSop, this.btnUploadPdf, this.lblPdfName });

            // ════════════════════════════════════════════════════════════
            // TAB: PRN EDITOR
            // ════════════════════════════════════════════════════════════
            this.tabPrn.Text = "PRN Editor";
            this.tabPrn.Padding = new System.Windows.Forms.Padding(10);

            this.cmbPrnPart.Location = new System.Drawing.Point(14, 14);
            this.cmbPrnPart.Size = new System.Drawing.Size(220, 26);
            this.cmbPrnPart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrnPart.SelectedIndexChanged += new System.EventHandler(this.cmbPrnPart_SelectedIndexChanged);

            this.txtPrnEditor.Location = new System.Drawing.Point(14, 48);
            this.txtPrnEditor.Size = new System.Drawing.Size(840, 420);
            this.txtPrnEditor.Multiline = true;
            this.txtPrnEditor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPrnEditor.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.txtPrnEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrnEditor.WordWrap = false;

            this.txtPrnPath.Location = new System.Drawing.Point(14, 476);
            this.txtPrnPath.Size = new System.Drawing.Size(600, 26);
            this.txtPrnPath.PlaceholderText = "PRN file path (optional)";

            this.btnPrnLoad.Location = new System.Drawing.Point(624, 476);
            this.btnPrnLoad.Size = new System.Drawing.Size(100, 26);
            this.btnPrnLoad.Text = "Load File";
            this.btnPrnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrnLoad.Click += new System.EventHandler(this.btnPrnLoad_Click);

            this.btnPrnSave.Location = new System.Drawing.Point(734, 476);
            this.btnPrnSave.Size = new System.Drawing.Size(120, 26);
            this.btnPrnSave.Text = "Save PRN";
            this.btnPrnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrnSave.BackColor = System.Drawing.Color.FromArgb(220, 240, 220);
            this.btnPrnSave.Click += new System.EventHandler(this.btnPrnSave_Click);

            this.tabPrn.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.cmbPrnPart, this.txtPrnEditor, this.txtPrnPath,
                this.btnPrnLoad, this.btnPrnSave });

            // ════════════════════════════════════════════════════════════
            // TAB: DISPATCH ORDERS
            // ════════════════════════════════════════════════════════════
            this.tabDispatch.Text = "Dispatch Orders";
            this.tabDispatch.Padding = new System.Windows.Forms.Padding(10);

            // ListView
            this.lstDispatchOrders.Location = new System.Drawing.Point(14, 14);
            this.lstDispatchOrders.Size = new System.Drawing.Size(856, 260);
            this.lstDispatchOrders.View = System.Windows.Forms.View.Details;
            this.lstDispatchOrders.FullRowSelect = true;
            this.lstDispatchOrders.GridLines = true;
            this.lstDispatchOrders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstDispatchOrders.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstDispatchOrders.SelectedIndexChanged += new System.EventHandler(this.lstDispatchOrders_SelectedIndexChanged);

            this.colOrderNo.Text = "Order No"; this.colOrderNo.Width = 160;
            this.colCustomer.Text = "Customer"; this.colCustomer.Width = 160;
            this.colPart.Text = "Part"; this.colPart.Width = 140;
            this.colQty.Text = "Qty"; this.colQty.Width = 60;
            this.colScanned.Text = "Scanned"; this.colScanned.Width = 80;
            this.colStatus.Text = "Status"; this.colStatus.Width = 90;
            this.colDue.Text = "Due Date"; this.colDue.Width = 100;

            this.lstDispatchOrders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colOrderNo, this.colCustomer, this.colPart,
                this.colQty, this.colScanned, this.colStatus, this.colDue });

            // Form fields
            int dx = 14, dy = 286;
            this.lblOrderCustomer.Location = new System.Drawing.Point(dx, dy);
            this.lblOrderCustomer.AutoSize = true;
            this.lblOrderCustomer.Text = "Customer Name:";
            this.txtOrderCustomer.Location = new System.Drawing.Point(dx, dy + 18);
            this.txtOrderCustomer.Size = new System.Drawing.Size(160, 26);
            this.txtOrderCustomer.PlaceholderText = "Customer";

            this.lblOrderPart.Location = new System.Drawing.Point(dx + 180, dy);
            this.lblOrderPart.AutoSize = true;
            this.lblOrderPart.Text = "Part Name:";
            this.txtOrderPart.Location = new System.Drawing.Point(dx + 180, dy + 18);
            this.txtOrderPart.Size = new System.Drawing.Size(160, 26);
            this.txtOrderPart.PlaceholderText = "Part name";

            this.lblOrderQRRef.Location = new System.Drawing.Point(dx + 360, dy);
            this.lblOrderQRRef.AutoSize = true;
            this.lblOrderQRRef.Text = "QR Reference (part match string):";
            this.txtOrderQRRef.Location = new System.Drawing.Point(dx + 360, dy + 18);
            this.txtOrderQRRef.Size = new System.Drawing.Size(220, 26);
            this.txtOrderQRRef.PlaceholderText = "Exact or partial QR value";

            this.lblOrderQty.Location = new System.Drawing.Point(dx + 600, dy);
            this.lblOrderQty.AutoSize = true;
            this.lblOrderQty.Text = "Qty:";
            this.txtOrderQty.Location = new System.Drawing.Point(dx + 600, dy + 18);
            this.txtOrderQty.Size = new System.Drawing.Size(70, 26);
            this.txtOrderQty.PlaceholderText = "Qty";

            this.lblOrderDue.Location = new System.Drawing.Point(dx + 690, dy);
            this.lblOrderDue.AutoSize = true;
            this.lblOrderDue.Text = "Due Date:";
            this.dtpOrderDue.Location = new System.Drawing.Point(dx + 690, dy + 18);
            this.dtpOrderDue.Size = new System.Drawing.Size(140, 26);
            this.dtpOrderDue.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOrderDue.Value = System.DateTime.Today.AddDays(1);

            // Action buttons row
            this.btnCreateOrder.Location = new System.Drawing.Point(dx, dy + 54);
            this.btnCreateOrder.Size = new System.Drawing.Size(130, 34);
            this.btnCreateOrder.Text = "➕ Create Token";
            this.btnCreateOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateOrder.BackColor = System.Drawing.Color.FromArgb(200, 230, 200);
            this.btnCreateOrder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCreateOrder.Click += new System.EventHandler(this.btnCreateOrder_Click);

            this.btnEditOrder.Location = new System.Drawing.Point(dx + 140, dy + 54);
            this.btnEditOrder.Size = new System.Drawing.Size(120, 34);
            this.btnEditOrder.Text = "✏ Save Edit";
            this.btnEditOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditOrder.BackColor = System.Drawing.Color.FromArgb(220, 235, 255);
            this.btnEditOrder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEditOrder.Enabled = false;
            this.btnEditOrder.Click += new System.EventHandler(this.btnEditOrder_Click);

            this.btnNewOrder.Location = new System.Drawing.Point(dx + 270, dy + 54);
            this.btnNewOrder.Size = new System.Drawing.Size(110, 34);
            this.btnNewOrder.Text = "🗒 New Token";
            this.btnNewOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewOrder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnNewOrder.Click += new System.EventHandler(this.btnNewOrder_Click);

            this.btnDeleteOrder.Location = new System.Drawing.Point(dx + 390, dy + 54);
            this.btnDeleteOrder.Size = new System.Drawing.Size(90, 34);
            this.btnDeleteOrder.Text = "🗑 Delete";
            this.btnDeleteOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteOrder.BackColor = System.Drawing.Color.FromArgb(255, 220, 220);
            this.btnDeleteOrder.Click += new System.EventHandler(this.btnDeleteOrder_Click);

            this.btnRefreshOrders.Location = new System.Drawing.Point(dx + 490, dy + 54);
            this.btnRefreshOrders.Size = new System.Drawing.Size(80, 34);
            this.btnRefreshOrders.Text = "↻ Refresh";
            this.btnRefreshOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshOrders.Click += new System.EventHandler(this.btnRefreshOrders_Click);

            this.tabDispatch.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lstDispatchOrders,
                this.lblOrderCustomer, this.txtOrderCustomer,
                this.lblOrderPart,     this.txtOrderPart,
                this.lblOrderQRRef,    this.txtOrderQRRef,
                this.lblOrderQty,      this.txtOrderQty,
                this.lblOrderDue,      this.dtpOrderDue,
                this.btnCreateOrder,   this.btnEditOrder,   this.btnNewOrder,
                this.btnDeleteOrder,   this.btnRefreshOrders });

            // ════════════════════════════════════════════════════════════
            // TAB: CUSTOMER PRN
            // ════════════════════════════════════════════════════════════
            this.tabCustomerPrn.Text = "Customer PRN";
            this.tabCustomerPrn.Padding = new System.Windows.Forms.Padding(10);

            this.cmbCustomerPrn.Location = new System.Drawing.Point(14, 14);
            this.cmbCustomerPrn.Size = new System.Drawing.Size(260, 26);
            this.cmbCustomerPrn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomerPrn.SelectedIndexChanged += new System.EventHandler(this.cmbCustomerPrn_SelectedIndexChanged);

            this.btnRefreshCustomerPrn.Location = new System.Drawing.Point(284, 14);
            this.btnRefreshCustomerPrn.Size = new System.Drawing.Size(80, 26);
            this.btnRefreshCustomerPrn.Text = "Refresh";
            this.btnRefreshCustomerPrn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshCustomerPrn.Click += new System.EventHandler(this.btnRefreshCustomerPrn_Click);

            // Token reference help
            var lblPrnHelp = new System.Windows.Forms.Label();
            lblPrnHelp.Location = new System.Drawing.Point(380, 14);
            lblPrnHelp.AutoSize = false;
            lblPrnHelp.Size = new System.Drawing.Size(480, 28);
            lblPrnHelp.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            lblPrnHelp.ForeColor = System.Drawing.Color.FromArgb(80, 80, 130);
            lblPrnHelp.Text = "Tokens: {PartNumber} {PartName} {CustomerName} {CustomerLocation} {Quantity} {OperatorName} {InspectorName} {InvoiceNo} {HodekPartNo} {MFGDate}";

            this.txtCustomerPrnEditor.Location = new System.Drawing.Point(14, 48);
            this.txtCustomerPrnEditor.Size = new System.Drawing.Size(840, 420);
            this.txtCustomerPrnEditor.Multiline = true;
            this.txtCustomerPrnEditor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCustomerPrnEditor.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.txtCustomerPrnEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustomerPrnEditor.WordWrap = false;

            this.txtCustomerPrnPath.Location = new System.Drawing.Point(14, 476);
            this.txtCustomerPrnPath.Size = new System.Drawing.Size(600, 26);
            this.txtCustomerPrnPath.PlaceholderText = "PRN file path (optional)";

            this.btnCustomerPrnLoad.Location = new System.Drawing.Point(624, 476);
            this.btnCustomerPrnLoad.Size = new System.Drawing.Size(100, 26);
            this.btnCustomerPrnLoad.Text = "Load File";
            this.btnCustomerPrnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCustomerPrnLoad.Click += new System.EventHandler(this.btnCustomerPrnLoad_Click);

            this.btnCustomerPrnSave.Location = new System.Drawing.Point(734, 476);
            this.btnCustomerPrnSave.Size = new System.Drawing.Size(120, 26);
            this.btnCustomerPrnSave.Text = "Save PRN";
            this.btnCustomerPrnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCustomerPrnSave.BackColor = System.Drawing.Color.FromArgb(220, 240, 220);
            this.btnCustomerPrnSave.Click += new System.EventHandler(this.btnCustomerPrnSave_Click);

            this.tabCustomerPrn.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.cmbCustomerPrn, this.btnRefreshCustomerPrn, lblPrnHelp,
                this.txtCustomerPrnEditor, this.txtCustomerPrnPath,
                this.btnCustomerPrnLoad, this.btnCustomerPrnSave });

            // ════════════════════════════════════════════════════════════
            // TAB: PRINTER
            // ════════════════════════════════════════════════════════════
            this.tabPrinter.Text = "Printer";
            this.tabPrinter.Padding = new System.Windows.Forms.Padding(10);

            this.lblPrinterName.Location = new System.Drawing.Point(14, 20);
            this.lblPrinterName.AutoSize = true;
            this.lblPrinterName.Text = "Printer share name:";

            this.txtPrinterName.Location = new System.Drawing.Point(14, 42);
            this.txtPrinterName.Size = new System.Drawing.Size(260, 26);
            this.txtPrinterName.PlaceholderText = "e.g. TSC_TE244";

            this.btnSavePrinter.Location = new System.Drawing.Point(14, 78);
            this.btnSavePrinter.Size = new System.Drawing.Size(120, 32);
            this.btnSavePrinter.Text = "Save";
            this.btnSavePrinter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePrinter.BackColor = System.Drawing.Color.FromArgb(220, 240, 220);
            this.btnSavePrinter.Click += new System.EventHandler(this.btnSavePrinter_Click);

            this.tabPrinter.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblPrinterName, this.txtPrinterName, this.btnSavePrinter });

            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ── control declarations ──────────────────────────────────────────
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabUsers, tabEmail, tabShift;
        private System.Windows.Forms.TabPage tabParts, tabPrn;
        private System.Windows.Forms.TabPage tabDispatch, tabCustomerPrn, tabPrinter;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.TextBox txtUser, txtPass;
        private System.Windows.Forms.Button btnAdd, btnDelete;
        private System.Windows.Forms.Label lblUserName, lblUserPass;
        private System.Windows.Forms.Label lblSender, lblPassword, lblReceiver;
        private System.Windows.Forms.TextBox txtSender, txtPassword, txtReceiver;
        private System.Windows.Forms.Button btnSaveEmail, btnTestMail, btnOpenCsv;
        private System.Windows.Forms.Label lblShiftA, lblShiftB, lblShiftC;
        private System.Windows.Forms.TextBox txtAStart, txtAEnd, txtTargetA;
        private System.Windows.Forms.TextBox txtBStart, txtBEnd, txtTargetB;
        private System.Windows.Forms.TextBox txtCStart, txtCEnd, txtTargetC;
        private System.Windows.Forms.Button btnSaveShift;
        private System.Windows.Forms.ListBox lstParts;
        private System.Windows.Forms.TextBox txtNewPart;
        private System.Windows.Forms.Button btnAddPart, btnDeletePart;
        private System.Windows.Forms.ComboBox cmbPartSop;
        private System.Windows.Forms.Button btnUploadPdf;
        private System.Windows.Forms.Label lblPdfName;
        private System.Windows.Forms.ComboBox cmbPrnPart;
        private System.Windows.Forms.TextBox txtPrnEditor, txtPrnPath;
        private System.Windows.Forms.Button btnPrnLoad, btnPrnSave;
        private System.Windows.Forms.ListView lstDispatchOrders;
        private System.Windows.Forms.ColumnHeader colOrderNo, colCustomer, colPart;
        private System.Windows.Forms.ColumnHeader colQty, colScanned, colStatus, colDue;
        private System.Windows.Forms.Label lblOrderCustomer, lblOrderPart;
        private System.Windows.Forms.Label lblOrderQRRef, lblOrderQty, lblOrderDue;
        private System.Windows.Forms.TextBox txtOrderCustomer, txtOrderPart;
        private System.Windows.Forms.TextBox txtOrderQRRef, txtOrderQty;
        private System.Windows.Forms.DateTimePicker dtpOrderDue;
        private System.Windows.Forms.Button btnCreateOrder, btnDeleteOrder, btnRefreshOrders;
        private System.Windows.Forms.Button btnEditOrder, btnNewOrder;
        private System.Windows.Forms.ComboBox cmbCustomerPrn;
        private System.Windows.Forms.TextBox txtCustomerPrnEditor, txtCustomerPrnPath;
        private System.Windows.Forms.Button btnCustomerPrnLoad, btnCustomerPrnSave, btnRefreshCustomerPrn;
        private System.Windows.Forms.Label lblPrinterName;
        private System.Windows.Forms.TextBox txtPrinterName;
        private System.Windows.Forms.Button btnSavePrinter;
    }
}