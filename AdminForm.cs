using System;
using System.IO;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public partial class AdminForm : Form
    {
        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeApp");
        private string sopFolder = "";
        private string _editingOrderNo = ""; // non-empty means we are in edit mode

        public AdminForm()
        {
            InitializeComponent();
            sopFolder = Path.Combine(baseFolder, "SOP");
            if (!Directory.Exists(sopFolder)) Directory.CreateDirectory(sopFolder);
            LoadUsers();
            LoadEmailSettings();
            LoadShiftSettings();
            LoadParts();
            LoadPrinterConfig();
            LoadPrnEditor();
            LoadDispatchOrders();
            LoadCustomerPrnEditor();
        }

        // ===== USER =====

        private void LoadUsers()
        {
            lstUsers.Items.Clear();
            foreach (var u in DatabaseHelper.GetUsers())
                lstUsers.Items.Add(u);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();
            if (user == "" || pass == "")
            { MessageBox.Show("Enter username & password"); return; }
            if (DatabaseHelper.AddUser(user, pass))
            { MessageBox.Show("User Added ✅"); txtUser.Clear(); txtPass.Clear(); LoadUsers(); }
            else MessageBox.Show("User already exists ❌");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem == null) return;
            string user = lstUsers.SelectedItem?.ToString() ?? "";
            if (user == "admin") { MessageBox.Show("Admin cannot be deleted ❌"); return; }
            if (MessageBox.Show($"Delete '{user}'?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            { DatabaseHelper.DeleteUser(user); MessageBox.Show("User Deleted ✅"); LoadUsers(); }
        }

        // ===== EMAIL =====

        private void LoadEmailSettings()
        {
            var (s, p, r) = DatabaseHelper.GetEmailSettings();
            txtSender.Text = s; txtPassword.Text = p; txtReceiver.Text = r;
        }

        private void btnSaveEmail_Click(object sender, EventArgs e)
        {
            DatabaseHelper.SaveEmailSettings(
                txtSender.Text.Trim(), txtPassword.Text.Trim(), txtReceiver.Text.Trim());
            MessageBox.Show("Email Settings Saved ✅");
        }

        // ===== SHIFT =====

        private void LoadShiftSettings()
        {
            var shifts = DatabaseHelper.GetShifts();
            foreach (var s in shifts)
            {
                if (s.shift == "A") { txtAStart.Text = s.start.ToString(@"hh\:mm"); txtAEnd.Text = s.end.ToString(@"hh\:mm"); }
                else if (s.shift == "B") { txtBStart.Text = s.start.ToString(@"hh\:mm"); txtBEnd.Text = s.end.ToString(@"hh\:mm"); }
                else if (s.shift == "C") { txtCStart.Text = s.start.ToString(@"hh\:mm"); txtCEnd.Text = s.end.ToString(@"hh\:mm"); }
            }
            txtTargetA.Text = DatabaseHelper.GetShiftTarget("A").ToString();
            txtTargetB.Text = DatabaseHelper.GetShiftTarget("B").ToString();
            txtTargetC.Text = DatabaseHelper.GetShiftTarget("C").ToString();
        }

        private void btnSaveShift_Click(object sender, EventArgs e)
        {
            try
            {
                string aS = txtAStart.Text.Replace('.', ':'), aE = txtAEnd.Text.Replace('.', ':');
                string bS = txtBStart.Text.Replace('.', ':'), bE = txtBEnd.Text.Replace('.', ':');
                string cS = txtCStart.Text.Replace('.', ':'), cE = txtCEnd.Text.Replace('.', ':');
                TimeSpan.Parse(aS); TimeSpan.Parse(aE);
                TimeSpan.Parse(bS); TimeSpan.Parse(bE);
                TimeSpan.Parse(cS); TimeSpan.Parse(cE);
                DatabaseHelper.UpdateShift("A", aS, aE);
                DatabaseHelper.UpdateShift("B", bS, bE);
                DatabaseHelper.UpdateShift("C", cS, cE);
                int tA = int.TryParse(txtTargetA.Text, out int ta) ? ta : 0;
                int tB = int.TryParse(txtTargetB.Text, out int tb) ? tb : 0;
                int tC = int.TryParse(txtTargetC.Text, out int tc) ? tc : 0;
                DatabaseHelper.SaveShiftTarget("A", tA);
                DatabaseHelper.SaveShiftTarget("B", tB);
                DatabaseHelper.SaveShiftTarget("C", tC);
                foreach (Form f in Application.OpenForms)
                    if (f is Form1 mf) { mf.BeginInvoke(new Action(() => mf.RefreshShiftTarget())); break; }
                MessageBox.Show("Shift settings saved ✅");
            }
            catch { MessageBox.Show("Enter valid times (HH:mm) and numeric targets"); }
        }

        // ===== PART =====

        private void LoadParts()
        {
            lstParts.Items.Clear(); cmbPartSop.Items.Clear();
            foreach (var p in DatabaseHelper.GetParts())
            { lstParts.Items.Add(p); cmbPartSop.Items.Add(p); }
            if (cmbPartSop.Items.Count > 0) cmbPartSop.SelectedIndex = 0;
        }

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            string part = txtNewPart.Text.Trim();
            if (string.IsNullOrEmpty(part)) { MessageBox.Show("Enter part name"); return; }
            if (DatabaseHelper.AddPart(part))
            { MessageBox.Show("Part Added ✅"); txtNewPart.Clear(); LoadParts(); LoadPrnEditor(); }
            else MessageBox.Show("Part already exists ❌");
        }

        private void btnDeletePart_Click(object sender, EventArgs e)
        {
            if (lstParts.SelectedItem == null) return;
            string part = lstParts.SelectedItem?.ToString() ?? "";
            if (MessageBox.Show($"Delete part '{part}'?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            { DatabaseHelper.DeletePart(part); MessageBox.Show("Part Deleted ✅"); LoadParts(); LoadPrnEditor(); }
        }

        // ===== SOP =====

        private void btnUploadPdf_Click(object sender, EventArgs e)
        {
            if (cmbPartSop.SelectedItem == null) { MessageBox.Show("Select a part first ❌"); return; }
            string selectedPart = cmbPartSop.SelectedItem.ToString() ?? "";
            var ofd = new OpenFileDialog { Filter = "PDF Files (*.pdf)|*.pdf" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string dest = Path.Combine(sopFolder, selectedPart + "_" + Path.GetFileName(ofd.FileName));
                File.Copy(ofd.FileName, dest, true);
                DatabaseHelper.SavePdfPath(selectedPart, dest);
                lblPdfName.Text = Path.GetFileName(dest);
                MessageBox.Show($"SOP uploaded for '{selectedPart}' ✅");
                foreach (Form f in Application.OpenForms)
                    if (f is Form1 mf) { mf.BeginInvoke(new Action(() => mf.LoadPDF())); break; }
            }
        }

        // ===== PRINTER CONFIG =====

        private void LoadPrinterConfig()
        {
            txtPrinterName.Text = DatabaseHelper.GetConfig("PrinterShareName");
        }

        private void btnSavePrinter_Click(object sender, EventArgs e)
        {
            string name = txtPrinterName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { MessageBox.Show("Enter printer share name"); return; }
            DatabaseHelper.SaveConfig("PrinterShareName", name);
            MessageBox.Show("Printer name saved ✅");
        }

        // ===== PRN EDITOR =====

        private void LoadPrnEditor()
        {
            cmbPrnPart.Items.Clear();
            foreach (var p in DatabaseHelper.GetParts()) cmbPrnPart.Items.Add(p);
            if (cmbPrnPart.Items.Count > 0) cmbPrnPart.SelectedIndex = 0;
            else { txtPrnEditor.Text = ""; txtPrnPath.Text = ""; }
        }

        private void cmbPrnPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            string part = cmbPrnPart.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(part)) return;
            string content = DatabaseHelper.GetPrnContent(part);
            string path = DatabaseHelper.GetPrnPath(part);
            txtPrnEditor.Text = string.IsNullOrWhiteSpace(content)
                ? $"// No PRN configured for '{part}' yet.\r\n// Write or load a PRN file, then click Save.\r\n"
                : content;
            txtPrnPath.Text = path;
        }

        private void btnPrnLoad_Click(object sender, EventArgs e)
        {
            string path = txtPrnPath.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                var ofd = new OpenFileDialog { Filter = "PRN Files (*.prn)|*.prn|All Files (*.*)|*.*" };
                if (ofd.ShowDialog() == DialogResult.OK) { txtPrnPath.Text = ofd.FileName; path = ofd.FileName; }
            }
            if (File.Exists(path)) { txtPrnEditor.Text = File.ReadAllText(path, System.Text.Encoding.ASCII); MessageBox.Show("PRN file loaded ✅"); }
            else MessageBox.Show("File not found ❌");
        }

        private void btnPrnSave_Click(object sender, EventArgs e)
        {
            string part = cmbPrnPart.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(part)) { MessageBox.Show("Select a part ❌"); return; }
            var lines = txtPrnEditor.Text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            string content = string.Join("\r\n",
                System.Linq.Enumerable.Where(lines, l => !l.TrimStart().StartsWith("//"))) + "\r\n";
            content = content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
            string path = txtPrnPath.Text.Trim();
            if (!string.IsNullOrEmpty(path))
            {
                try { File.WriteAllText(path, content, System.Text.Encoding.ASCII); }
                catch (Exception ex) { MessageBox.Show("File save error: " + ex.Message); return; }
            }
            DatabaseHelper.SavePrnConfig(part, content, path);
            MessageBox.Show($"PRN saved for '{part}' ✅");
            txtPrnEditor.Text = DatabaseHelper.GetPrnContent(part);
        }

        // ===== DISPATCH ORDERS =====

        private void LoadDispatchOrders()
        {
            lstDispatchOrders.Items.Clear();
            var orders = DatabaseHelper.GetDispatchOrders();
            foreach (var o in orders)
            {
                var item = new ListViewItem(o.OrderNo);
                item.SubItems.Add(o.CustomerName);
                item.SubItems.Add(o.PartName);
                item.SubItems.Add(o.QtyOrdered.ToString());
                item.SubItems.Add($"{o.QtyScanned} / {o.QtyOrdered}");
                item.SubItems.Add(o.Status);
                item.SubItems.Add(o.DueDate);
                item.ForeColor = GetDueColour(o.DueDate, o.Status);
                lstDispatchOrders.Items.Add(item);
            }
        }

        private System.Drawing.Color GetDueColour(string dueDateStr, string status)
        {
            if (status == "Done") return System.Drawing.Color.FromArgb(60, 160, 60);
            if (!DateTime.TryParse(dueDateStr, out DateTime due))
                return System.Drawing.Color.Gray;
            int daysLeft = (due.Date - DateTime.Today).Days;
            int yellow = int.TryParse(DatabaseHelper.GetConfig("YellowDaysBeforeDue"), out int y) ? y : 1;
            int red = int.TryParse(DatabaseHelper.GetConfig("RedDaysBeforeDue"), out int r) ? r : 0;
            if (daysLeft <= red) return System.Drawing.Color.FromArgb(210, 50, 50);
            if (daysLeft <= yellow) return System.Drawing.Color.FromArgb(200, 140, 0);
            return System.Drawing.Color.FromArgb(30, 130, 30);
        }

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            string customer = txtOrderCustomer.Text.Trim();
            string partName = txtOrderPart.Text.Trim();
            string qrRef = txtOrderQRRef.Text.Trim();
            string dueDate = dtpOrderDue.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(customer) || string.IsNullOrEmpty(partName) ||
                string.IsNullOrEmpty(qrRef))
            { MessageBox.Show("Fill in Customer, Part Name and QR Reference ❌"); return; }

            if (!int.TryParse(txtOrderQty.Text.Trim(), out int qty) || qty <= 0)
            { MessageBox.Show("Enter a valid quantity ❌"); return; }

            string orderNo = DatabaseHelper.GenerateOrderNo();
            string createdBy = "";
            foreach (Form f in Application.OpenForms)
                if (f is Form1 mf) { createdBy = mf.CurrentUser; break; }
            if (string.IsNullOrEmpty(createdBy)) createdBy = "admin";

            DatabaseHelper.SaveDispatchOrder(orderNo, dueDate, customer, partName, qrRef, qty, createdBy);

            MessageBox.Show($"Dispatch Token Created ✅\nOrder No: {orderNo}", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtOrderCustomer.Clear(); txtOrderPart.Clear();
            txtOrderQRRef.Clear(); txtOrderQty.Clear();
            dtpOrderDue.Value = DateTime.Today.AddDays(1);

            LoadDispatchOrders();

            // Refresh tokens on main form
            foreach (Form f in Application.OpenForms)
                if (f is Form1 mf) { mf.BeginInvoke(new Action(() => mf.LoadDispatchTokens())); break; }
        }

        private void btnDeleteOrder_Click(object sender, EventArgs e)
        {
            if (lstDispatchOrders.SelectedItems.Count == 0) return;
            string orderNo = lstDispatchOrders.SelectedItems[0].Text;
            var order = DatabaseHelper.GetDispatchOrder(orderNo);
            if (order == null) return;
            if (order.Status == "InProgress")
            { MessageBox.Show("Cannot delete an order that is currently In Progress ❌"); return; }
            if (MessageBox.Show($"Delete order '{orderNo}'?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteDispatchOrder(orderNo);
                LoadDispatchOrders();
                foreach (Form f in Application.OpenForms)
                    if (f is Form1 mf) { mf.BeginInvoke(new Action(() => mf.LoadDispatchTokens())); break; }
            }
        }

        private void btnRefreshOrders_Click(object sender, EventArgs e) => LoadDispatchOrders();

        // ── Token edit support ─────────────────────────────────────────────

        /// <summary>When a row is selected, populate form fields for editing.</summary>
        private void lstDispatchOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDispatchOrders.SelectedItems.Count == 0)
            {
                ResetEditMode();
                return;
            }

            string orderNo = lstDispatchOrders.SelectedItems[0].Text;
            var order = DatabaseHelper.GetDispatchOrder(orderNo);
            if (order == null) return;

            // Populate fields
            txtOrderCustomer.Text = order.CustomerName;
            txtOrderPart.Text     = order.PartName;
            txtOrderQRRef.Text    = order.QRReference;
            txtOrderQty.Text      = order.QtyOrdered.ToString();
            if (DateTime.TryParse(order.DueDate, out DateTime due))
                dtpOrderDue.Value = due;

            // Enter edit mode
            _editingOrderNo        = orderNo;
            btnEditOrder.Enabled   = true;
            btnCreateOrder.Enabled = false;
        }

        /// <summary>Save edits to selected order.</summary>
        private void btnEditOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_editingOrderNo)) return;

            string customer = txtOrderCustomer.Text.Trim();
            string partName = txtOrderPart.Text.Trim();
            string qrRef    = txtOrderQRRef.Text.Trim();
            string dueDate  = dtpOrderDue.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(customer) || string.IsNullOrEmpty(partName) || string.IsNullOrEmpty(qrRef))
            { MessageBox.Show("Fill in Customer, Part Name and QR Reference ❌"); return; }

            if (!int.TryParse(txtOrderQty.Text.Trim(), out int qty) || qty <= 0)
            { MessageBox.Show("Enter a valid quantity ❌"); return; }

            DatabaseHelper.UpdateDispatchOrder(_editingOrderNo, customer, partName, qrRef, qty, dueDate);

            MessageBox.Show($"Order {_editingOrderNo} updated ✅", "Saved",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            ResetEditMode();
            LoadDispatchOrders();

            foreach (Form f in Application.OpenForms)
                if (f is Form1 mf) { mf.BeginInvoke(new Action(() => mf.LoadDispatchTokens())); break; }
        }

        /// <summary>Clear fields and return to Create mode.</summary>
        private void btnNewOrder_Click(object sender, EventArgs e) => ResetEditMode();

        private void ResetEditMode()
        {
            _editingOrderNo        = "";
            btnEditOrder.Enabled   = false;
            btnCreateOrder.Enabled = true;
            txtOrderCustomer.Clear(); txtOrderPart.Clear();
            txtOrderQRRef.Clear();    txtOrderQty.Clear();
            dtpOrderDue.Value = DateTime.Today.AddDays(1);
            lstDispatchOrders.SelectedItems.Clear();
        }

        // ===== CUSTOMER PRN EDITOR =====

        private void LoadCustomerPrnEditor()
        {
            cmbCustomerPrn.Items.Clear();
            foreach (var c in DatabaseHelper.GetCustomers())
                cmbCustomerPrn.Items.Add(c);
            if (cmbCustomerPrn.Items.Count > 0) cmbCustomerPrn.SelectedIndex = 0;
        }

        private void cmbCustomerPrn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string customer = cmbCustomerPrn.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(customer)) return;
            string content = DatabaseHelper.GetCustomerPrnContent(customer);
            string path = DatabaseHelper.GetCustomerPrnPath(customer);
            txtCustomerPrnEditor.Text = string.IsNullOrWhiteSpace(content)
                ? $"// No PRN configured for '{customer}' yet.\r\n"
                : content;
            txtCustomerPrnPath.Text = path;
        }

        private void btnCustomerPrnLoad_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "PRN Files (*.prn)|*.prn|All Files (*.*)|*.*" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtCustomerPrnPath.Text = ofd.FileName;
                txtCustomerPrnEditor.Text = File.ReadAllText(ofd.FileName, System.Text.Encoding.ASCII);
                MessageBox.Show("PRN loaded ✅");
            }
        }

        private void btnCustomerPrnSave_Click(object sender, EventArgs e)
        {
            string customer = cmbCustomerPrn.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(customer)) { MessageBox.Show("Select a customer ❌"); return; }
            var lines = txtCustomerPrnEditor.Text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            string content = string.Join("\r\n",
                System.Linq.Enumerable.Where(lines, l => !l.TrimStart().StartsWith("//"))) + "\r\n";
            content = content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
            string path = txtCustomerPrnPath.Text.Trim();
            if (!string.IsNullOrEmpty(path))
            {
                try { File.WriteAllText(path, content, System.Text.Encoding.ASCII); }
                catch (Exception ex) { MessageBox.Show("File save error: " + ex.Message); return; }
            }
            DatabaseHelper.SaveCustomerPrn(customer, content, path);
            MessageBox.Show($"Customer PRN saved for '{customer}' ✅");
        }

        private void btnRefreshCustomerPrn_Click(object sender, EventArgs e) => LoadCustomerPrnEditor();

        // ===== MAIL & CSV =====

        private void btnOpenCsv_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", baseFolder);
        }

        private void btnTestMail_Click(object sender, EventArgs e)
        {
            string file = "";
            foreach (Form f in Application.OpenForms)
                if (f is Form1 mf) { file = mf.GetCsvPath(); break; }
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            { MessageBox.Show("No CSV found yet!"); return; }
            EmailHelper.SendEmailAsync(file, "Test Report");
            MessageBox.Show("Sending in background! ✅");
        }
    }
}