namespace BarcodeBartenderApp
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelUser, panelEmail, panelShift, panelSOP, panelPart;
        private Label lblUserTitle, lblEmailTitle, lblShiftTitle, lblSOPTitle, lblPartTitle;
        private TextBox txtUser, txtPass, txtSender, txtPassword, txtReceiver;
        private TextBox txtAStart, txtAEnd, txtBStart, txtBEnd, txtCStart, txtCEnd;
        private TextBox txtNewPart;
        private Button btnAdd, btnDelete, btnSaveEmail, btnSaveShift;
        private Button btnAddPart, btnDeletePart, btnUploadPdf;
        private ListBox lstUsers, lstParts;
        private ComboBox cmbPartSop;
        private Label lblPdfName;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Panel";
            this.Size = new System.Drawing.Size(1150, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);

            var titleColor = System.Drawing.Color.FromArgb(0, 120, 215);

            // ===== USER PANEL =====
            panelUser = CreateCard(20, 20, 320, 320);
            lblUserTitle = CreateTitle("👤 User Management", titleColor);
            txtUser = new TextBox() { Top = 45, Left = 20, Width = 270, PlaceholderText = "Username" };
            txtPass = new TextBox() { Top = 80, Left = 20, Width = 270, PlaceholderText = "Password", PasswordChar = '*' };
            btnAdd = CreateButton("Add User", 120, 20, System.Drawing.Color.FromArgb(40, 167, 69));
            btnAdd.Click += btnAdd_Click;
            btnDelete = CreateButton("Delete User", 120, 130, System.Drawing.Color.FromArgb(220, 53, 69));
            btnDelete.Click += btnDelete_Click;
            lstUsers = new ListBox() { Top = 165, Left = 20, Width = 270, Height = 130 };
            panelUser.Controls.AddRange(new Control[] { lblUserTitle, txtUser, txtPass, btnAdd, btnDelete, lstUsers });

            // ===== EMAIL PANEL =====
            panelEmail = CreateCard(360, 20, 340, 200);
            lblEmailTitle = CreateTitle("📧 Email Configuration", titleColor);
            txtSender = new TextBox() { Top = 45, Left = 20, Width = 290, PlaceholderText = "Sender Gmail" };
            txtPassword = new TextBox() { Top = 80, Left = 20, Width = 290, PlaceholderText = "App Password" };
            txtReceiver = new TextBox() { Top = 115, Left = 20, Width = 290, PlaceholderText = "Receiver Email" };
            btnSaveEmail = CreateButton("Save Email Settings", 155, 20, titleColor);
            btnSaveEmail.Width = 200;
            btnSaveEmail.Click += btnSaveEmail_Click;
            panelEmail.Controls.AddRange(new Control[] { lblEmailTitle, txtSender, txtPassword, txtReceiver, btnSaveEmail });

            // ===== SHIFT PANEL =====
            panelShift = CreateCard(720, 20, 390, 320);
            lblShiftTitle = CreateTitle("🕐 Shift Management", titleColor);

            panelShift.Controls.Add(lblShiftTitle);
            AddShiftRow(panelShift, "Shift A:", 45, out txtAStart, out txtAEnd);
            AddShiftRow(panelShift, "Shift B:", 90, out txtBStart, out txtBEnd);
            AddShiftRow(panelShift, "Shift C:", 135, out txtCStart, out txtCEnd);

            btnSaveShift = CreateButton("Save Shifts", 180, 20, titleColor);
            btnSaveShift.Click += btnSaveShift_Click;
            panelShift.Controls.Add(btnSaveShift);

            // ===== PART PANEL =====
            panelPart = CreateCard(20, 360, 480, 300);
            lblPartTitle = CreateTitle("🔩 Part Management", titleColor);
            txtNewPart = new TextBox() { Top = 45, Left = 20, Width = 250, PlaceholderText = "New Part Name" };
            btnAddPart = CreateButton("Add Part", 45, 285, System.Drawing.Color.FromArgb(40, 167, 69));
            btnAddPart.Click += btnAddPart_Click;
            btnDeletePart = CreateButton("Delete Part", 45, 390, System.Drawing.Color.FromArgb(220, 53, 69));
            btnDeletePart.Click += btnDeletePart_Click;
            lstParts = new ListBox() { Top = 90, Left = 20, Width = 430, Height = 190 };
            panelPart.Controls.AddRange(new Control[] { lblPartTitle, txtNewPart, btnAddPart, btnDeletePart, lstParts });

            // ===== SOP PANEL =====
            panelSOP = CreateCard(520, 360, 600, 300);
            lblSOPTitle = CreateTitle("📄 SOP Management (Per Part)", titleColor);

            var lblSelectPart = new Label() { Text = "Select Part:", Top = 45, Left = 20, AutoSize = true };
            cmbPartSop = new ComboBox() { Top = 42, Left = 110, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            btnUploadPdf = CreateButton("Upload PDF for Part", 80, 20, titleColor);
            btnUploadPdf.Width = 200;
            btnUploadPdf.Click += btnUploadPdf_Click;

            lblPdfName = new Label() { Top = 125, Left = 20, Width = 550, ForeColor = System.Drawing.Color.Gray };

            panelSOP.Controls.AddRange(new Control[] { lblSOPTitle, lblSelectPart, cmbPartSop, btnUploadPdf, lblPdfName });

            Controls.AddRange(new Control[] { panelUser, panelEmail, panelShift, panelPart, panelSOP });
        }

        private Panel CreateCard(int x, int y, int w, int h)
        {
            return new Panel
            {
                Left = x,
                Top = y,
                Width = w,
                Height = h,
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Label CreateTitle(string text, System.Drawing.Color color)
        {
            return new Label
            {
                Text = text,
                Top = 10,
                Left = 15,
                AutoSize = true,
                ForeColor = color,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
        }

        private Button CreateButton(string text, int top, int left, System.Drawing.Color backColor)
        {
            return new Button
            {
                Text = text,
                Top = top,
                Left = left,
                BackColor = backColor,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 30,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
        }

        private void AddShiftRow(Panel panel, string label, int top, out TextBox start, out TextBox end)
        {
            panel.Controls.Add(new Label() { Text = label, Top = top + 3, Left = 15, AutoSize = true });
            start = new TextBox() { Top = top, Left = 80, Width = 80, Text = "00:00" };
            end = new TextBox() { Top = top, Left = 200, Width = 80, Text = "00:00" };
            panel.Controls.Add(new Label() { Text = "to", Top = top + 3, Left = 170, AutoSize = true });
            panel.Controls.Add(start);
            panel.Controls.Add(end);
        }
    }
}