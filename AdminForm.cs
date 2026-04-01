using System;
using System.IO;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public partial class AdminForm : Form
    {
        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "BarcodeApp");
        private string sopFolder = "";

        public AdminForm()
        {
            InitializeComponent();
            sopFolder = Path.Combine(baseFolder, "SOP");
            if (!Directory.Exists(sopFolder))
                Directory.CreateDirectory(sopFolder);
            LoadUsers();
            LoadEmailSettings();
            LoadShiftSettings();
            LoadParts();
            LoadPrinterConfig();
            LoadPrnEditor();
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
            {
                MessageBox.Show("User Added ✅");
                txtUser.Clear(); txtPass.Clear();
                LoadUsers();
            }
            else MessageBox.Show("User already exists ❌");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem == null) return;
            string user = lstUsers.SelectedItem?.ToString() ?? "";
            if (user == "admin")
            { MessageBox.Show("Admin cannot be deleted ❌"); return; }
            if (MessageBox.Show($"Delete '{user}'?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteUser(user);
                MessageBox.Show("User Deleted ✅");
                LoadUsers();
            }
        }

        // ===== EMAIL =====

        private void LoadEmailSettings()
        {
            var (s, p, r) = DatabaseHelper.GetEmailSettings();
            txtSender.Text = s;
            txtPassword.Text = p;
            txtReceiver.Text = r;
        }

        private void btnSaveEmail_Click(object sender, EventArgs e)
        {
            DatabaseHelper.SaveEmailSettings(
                txtSender.Text.Trim(),
                txtPassword.Text.Trim(),
                txtReceiver.Text.Trim());
            MessageBox.Show("Email Settings Saved ✅");
        }

        // ===== SHIFT =====

        private void LoadShiftSettings()
        {
            var shifts = DatabaseHelper.GetShifts();
            foreach (var s in shifts)
            {
                if (s.shift == "A")
                { txtAStart.Text = s.start.ToString(@"hh\:mm"); txtAEnd.Text = s.end.ToString(@"hh\:mm"); }
                else if (s.shift == "B")
                { txtBStart.Text = s.start.ToString(@"hh\:mm"); txtBEnd.Text = s.end.ToString(@"hh\:mm"); }
                else if (s.shift == "C")
                { txtCStart.Text = s.start.ToString(@"hh\:mm"); txtCEnd.Text = s.end.ToString(@"hh\:mm"); }
            }
            txtTargetA.Text = DatabaseHelper.GetShiftTarget("A").ToString();
            txtTargetB.Text = DatabaseHelper.GetShiftTarget("B").ToString();
            txtTargetC.Text = DatabaseHelper.GetShiftTarget("C").ToString();
        }

        private void btnSaveShift_Click(object sender, EventArgs e)
        {
            try
            {
                string aS = txtAStart.Text.Replace('.', ':');
                string aE = txtAEnd.Text.Replace('.', ':');
                string bS = txtBStart.Text.Replace('.', ':');
                string bE = txtBEnd.Text.Replace('.', ':');
                string cS = txtCStart.Text.Replace('.', ':');
                string cE = txtCEnd.Text.Replace('.', ':');

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

                MessageBox.Show("Shift settings saved ✅");
            }
            catch
            {
                MessageBox.Show("Enter valid times (HH:mm) and numeric targets");
            }
        }

        // ===== PART =====

        private void LoadParts()
        {
            lstParts.Items.Clear();
            cmbPartSop.Items.Clear();
            foreach (var p in DatabaseHelper.GetParts())
            {
                lstParts.Items.Add(p);
                cmbPartSop.Items.Add(p);
            }
            if (cmbPartSop.Items.Count > 0) cmbPartSop.SelectedIndex = 0;
        }

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            string part = txtNewPart.Text.Trim();
            if (string.IsNullOrEmpty(part))
            { MessageBox.Show("Enter part name"); return; }
            if (DatabaseHelper.AddPart(part))
            {
                MessageBox.Show("Part Added ✅");
                txtNewPart.Clear();
                LoadParts();
                LoadPrnEditor(); // refresh PRN dropdown too
            }
            else MessageBox.Show("Part already exists ❌");
        }

        private void btnDeletePart_Click(object sender, EventArgs e)
        {
            if (lstParts.SelectedItem == null) return;
            string part = lstParts.SelectedItem?.ToString() ?? "";
            if (MessageBox.Show($"Delete part '{part}'?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseHelper.DeletePart(part);
                MessageBox.Show("Part Deleted ✅");
                LoadParts();
                LoadPrnEditor();
            }
        }

        // ===== SOP =====

        private void btnUploadPdf_Click(object sender, EventArgs e)
        {
            if (cmbPartSop.SelectedItem == null)
            { MessageBox.Show("Select a part first ❌"); return; }
            string selectedPart = cmbPartSop.SelectedItem.ToString() ?? "";
            var ofd = new OpenFileDialog { Filter = "PDF Files (*.pdf)|*.pdf" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string dest = Path.Combine(sopFolder,
                    selectedPart + "_" + Path.GetFileName(ofd.FileName));
                File.Copy(ofd.FileName, dest, true);
                DatabaseHelper.SavePdfPath(selectedPart, dest);
                lblPdfName.Text = Path.GetFileName(dest);
                MessageBox.Show($"SOP uploaded for '{selectedPart}' ✅");
                foreach (Form f in Application.OpenForms)
                    if (f is Form1 mf)
                    { mf.BeginInvoke(new Action(() => mf.LoadPDF())); break; }
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
            if (string.IsNullOrEmpty(name))
            { MessageBox.Show("Enter printer share name"); return; }
            DatabaseHelper.SaveConfig("PrinterShareName", name);
            MessageBox.Show("Printer name saved ✅");
        }

        // ===== PRN EDITOR =====

        private void LoadPrnEditor()
        {
            cmbPrnPart.Items.Clear();
            foreach (var p in DatabaseHelper.GetParts())
                cmbPrnPart.Items.Add(p);
            if (cmbPrnPart.Items.Count > 0)
                cmbPrnPart.SelectedIndex = 0;
        }

        private void cmbPrnPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            string part = cmbPrnPart.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(part)) return;

            string content = DatabaseHelper.GetPrnContent(part);
            string path = DatabaseHelper.GetPrnPath(part);

            // Show default template if nothing saved yet
            if (string.IsNullOrWhiteSpace(content))
                content =
                    "SIZE 24 mm,8 mm\r\n" +
                    "GAP 1 mm,0 mm\r\n" +
                    "CLS\r\n" +
                    "QRCODE 5,5,L,3,A,0,\"{barcode}\"\r\n" +
                    "TEXT 45,5,\"2\",0,1,1,\"{barcode}\"\r\n" +
                    "TEXT 45,18,\"2\",0,1,1,\"{PartName}\"\r\n" +
                    "TEXT 45,31,\"2\",0,1,1,\"{serialNumber}\"\r\n" +
                    "PRINT 1\r\n";

            txtPrnEditor.Text = content;
            txtPrnPath.Text = path;
        }

        private void btnPrnLoad_Click(object sender, EventArgs e)
        {
            string path = txtPrnPath.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                var ofd = new OpenFileDialog { Filter = "PRN Files (*.prn)|*.prn|All Files (*.*)|*.*" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPrnPath.Text = ofd.FileName;
                    path = ofd.FileName;
                }
            }
            if (File.Exists(path))
            {
                txtPrnEditor.Text = File.ReadAllText(path);
                MessageBox.Show("PRN file loaded ✅");
            }
            else
                MessageBox.Show("File not found ❌");
        }

        private void btnPrnSave_Click(object sender, EventArgs e)
        {
            string part = cmbPrnPart.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(part)) { MessageBox.Show("Select a part ❌"); return; }

            string content = txtPrnEditor.Text;
            string path = txtPrnPath.Text.Trim();

            if (!string.IsNullOrEmpty(path))
            {
                try { File.WriteAllText(path, content); }
                catch (Exception ex) { MessageBox.Show("File save error: " + ex.Message); return; }
            }

            DatabaseHelper.SavePrnConfig(part, content, path);
            MessageBox.Show($"PRN saved for '{part}' ✅");
        }

        // ===== MAIL & CSV (moved from Form1) =====

        private void btnOpenCsv_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", baseFolder);
        }

        private void btnTestMail_Click(object sender, EventArgs e)
        {
            // Get CSV path from open Form1 instance
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
