using System;
using System.Windows.Forms;
using System.IO;

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
        }

        // ================= USER =================

        private void LoadUsers()
        {
            lstUsers.Items.Clear();
            foreach (var user in DatabaseHelper.GetUsers())
                lstUsers.Items.Add(user);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();

            if (user == "" || pass == "")
            {
                MessageBox.Show("Enter username & password");
                return;
            }

            if (DatabaseHelper.AddUser(user, pass))
            {
                MessageBox.Show("User Added ✅");
                txtUser.Clear();
                txtPass.Clear();
                LoadUsers();
            }
            else
            {
                MessageBox.Show("User already exists ❌");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem == null) return;

            string user = lstUsers.SelectedItem?.ToString() ?? "";

            if (user == "admin")
            {
                MessageBox.Show("Admin cannot be deleted ❌");
                return;
            }

            if (MessageBox.Show($"Delete user '{user}'?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteUser(user);
                MessageBox.Show("User Deleted ✅");
                LoadUsers();
            }
        }

        // ================= EMAIL =================

        private void LoadEmailSettings()
        {
            var (sender, password, receiver) = DatabaseHelper.GetEmailSettings();
            txtSender.Text = sender;
            txtPassword.Text = password;
            txtReceiver.Text = receiver;
        }

        private void btnSaveEmail_Click(object sender, EventArgs e)
        {
            DatabaseHelper.SaveEmailSettings(
                txtSender.Text.Trim(),
                txtPassword.Text.Trim(),
                txtReceiver.Text.Trim());
            MessageBox.Show("Email Settings Saved ✅");
        }

        // ================= SHIFT =================

        private void LoadShiftSettings()
        {
            var shifts = DatabaseHelper.GetShifts();
            foreach (var s in shifts)
            {
                if (s.shift == "A")
                {
                    txtAStart.Text = s.start.ToString(@"hh\:mm");
                    txtAEnd.Text = s.end.ToString(@"hh\:mm");
                }
                else if (s.shift == "B")
                {
                    txtBStart.Text = s.start.ToString(@"hh\:mm");
                    txtBEnd.Text = s.end.ToString(@"hh\:mm");
                }
                else if (s.shift == "C")
                {
                    txtCStart.Text = s.start.ToString(@"hh\:mm");
                    txtCEnd.Text = s.end.ToString(@"hh\:mm");
                }
            }
        }

        private void btnSaveShift_Click(object sender, EventArgs e)
        {
            try
            {
                string aStart = txtAStart.Text.Replace('.', ':');
                string aEnd = txtAEnd.Text.Replace('.', ':');
                string bStart = txtBStart.Text.Replace('.', ':');
                string bEnd = txtBEnd.Text.Replace('.', ':');
                string cStart = txtCStart.Text.Replace('.', ':');
                string cEnd = txtCEnd.Text.Replace('.', ':');

                TimeSpan.Parse(aStart);
                TimeSpan.Parse(aEnd);
                TimeSpan.Parse(bStart);
                TimeSpan.Parse(bEnd);
                TimeSpan.Parse(cStart);
                TimeSpan.Parse(cEnd);

                DatabaseHelper.UpdateShift("A", aStart, aEnd);
                DatabaseHelper.UpdateShift("B", bStart, bEnd);
                DatabaseHelper.UpdateShift("C", cStart, cEnd);

                MessageBox.Show("Shift timings updated ✅");
            }
            catch
            {
                MessageBox.Show("Enter time in HH:mm format (Example: 06:00)");
            }
        }

        // ================= PART =================

        private void LoadParts()
        {
            lstParts.Items.Clear();
            cmbPartSop.Items.Clear();

            foreach (var part in DatabaseHelper.GetParts())
            {
                lstParts.Items.Add(part);
                cmbPartSop.Items.Add(part);
            }

            if (cmbPartSop.Items.Count > 0)
                cmbPartSop.SelectedIndex = 0;
        }

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            string part = txtNewPart.Text.Trim();
            if (string.IsNullOrEmpty(part))
            {
                MessageBox.Show("Enter part name");
                return;
            }

            if (DatabaseHelper.AddPart(part))
            {
                MessageBox.Show("Part Added ✅");
                txtNewPart.Clear();
                LoadParts();
            }
            else
            {
                MessageBox.Show("Part already exists ❌");
            }
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
            }
        }

        // ================= SOP PDF =================

        private void btnUploadPdf_Click(object sender, EventArgs e)
        {
            if (cmbPartSop.SelectedItem == null)
            {
                MessageBox.Show("Select a part first ❌");
                return;
            }

            string selectedPart = cmbPartSop.SelectedItem.ToString() ?? "";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PDF Files (*.pdf)|*.pdf";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string destPath = Path.Combine(sopFolder,
                    selectedPart + "_" + Path.GetFileName(ofd.FileName));

                File.Copy(ofd.FileName, destPath, true);
                DatabaseHelper.SavePdfPath(selectedPart, destPath);

                lblPdfName.Text = Path.GetFileName(destPath);
                MessageBox.Show($"SOP uploaded for part '{selectedPart}' ✅");

                foreach (Form f in Application.OpenForms)
                {
                    if (f is Form1 mainForm)
                    {
                        mainForm.BeginInvoke(new Action(() => mainForm.LoadPDF()));
                        break;
                    }
                }
            }
        }
    }
}