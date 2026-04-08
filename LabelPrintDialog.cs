using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    /// <summary>
    /// Modal dialog shown when a dispatch order is completed or when reprinting a customer label.
    /// Operator fills in 10 fields; values replace tokens in the CustomerPrn template.
    /// </summary>
    public class LabelPrintDialog : Form
    {
        // ── inputs ───────────────────────────────────────────────────────
        private TextBox txtPartNumber    = new TextBox();
        private TextBox txtPartName      = new TextBox();
        private TextBox txtCustomerName  = new TextBox();
        private TextBox txtCustLocation  = new TextBox();
        private TextBox txtQuantity      = new TextBox();
        private TextBox txtOperatorName  = new TextBox();
        private TextBox txtInspectorName = new TextBox();
        private TextBox txtInvoiceNo     = new TextBox();
        private TextBox txtHodekPartNo   = new TextBox();
        private DateTimePicker dtpMfgDate = new DateTimePicker();

        private Button btnPrint  = new Button();
        private Button btnCancel = new Button();

        private readonly string _printerShareName;
        private readonly string _baseFolder;
        private readonly string _prnContent;
        private readonly string _orderNo;

        // ── constructor ──────────────────────────────────────────────────
        public LabelPrintDialog(DispatchOrder order,
            string operatorName, string inspectorName,
            string printerShareName, string baseFolder)
        {
            _printerShareName = printerShareName;
            _baseFolder       = baseFolder;
            _orderNo          = order.OrderNo;

            _prnContent = DatabaseHelper.GetCustomerPrnContent(order.CustomerName);
            if (string.IsNullOrWhiteSpace(_prnContent))
            {
                MessageBox.Show(
                    $"No PRN template configured for customer '{order.CustomerName}'.\n" +
                    "Go to Admin → Customer PRN to set up the template.",
                    "PRN Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Load += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
                return;
            }

            // Pre-fill known values
            txtPartName.Text      = order.PartName;
            txtCustomerName.Text  = order.CustomerName;
            txtQuantity.Text      = order.QtyScanned.ToString();
            txtOperatorName.Text  = operatorName;
            txtInspectorName.Text = inspectorName;

            BuildUI();
        }

        // ── UI builder ───────────────────────────────────────────────────
        private void BuildUI()
        {
            Text            = $"Print Customer Label — {_orderNo}";
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            BackColor       = Color.FromArgb(248, 250, 255);
            Font            = new Font("Segoe UI", 9.5F);

            // ── title banner ─────────────────────────────────────────────
            var banner = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 44,
                BackColor = Color.FromArgb(24, 48, 96)
            };
            var lblTitle = new Label
            {
                Text      = $"🖨  Customer Label — Order: {_orderNo}",
                Font      = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(12, 0, 0, 0)
            };
            banner.Controls.Add(lblTitle);
            Controls.Add(banner);

            // ── help label ───────────────────────────────────────────────
            var lblHelp = new Label
            {
                Text      = "Fields marked * are required. Fill in all details before printing.",
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(90, 90, 110),
                Location  = new Point(16, 54),
                AutoSize  = true
            };
            Controls.Add(lblHelp);

            // ── row builder ──────────────────────────────────────────────
            int y = 80;
            const int LBL_X = 16, LBL_W = 160, TXT_X = 182, TXT_W = 290;

            void AddRow(string caption, Control ctrl, bool required = false, bool readOnly = false)
            {
                var lbl = new Label
                {
                    Text      = caption + (required ? " *" : ""),
                    Location  = new Point(LBL_X, y + 4),
                    Size      = new Size(LBL_W, 22),
                    Font      = new Font("Segoe UI", 9F,
                                    required ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = required ? Color.FromArgb(160, 30, 0) : Color.FromArgb(50, 50, 70)
                };

                ctrl.Location  = new Point(TXT_X, y);
                ctrl.Size      = new Size(TXT_W, 28);
                ctrl.Font      = new Font("Segoe UI", 9F);

                if (ctrl is TextBox tb && readOnly)
                {
                    tb.ReadOnly  = true;
                    tb.BackColor = Color.FromArgb(235, 235, 235);
                    tb.ForeColor = Color.FromArgb(80, 80, 80);
                }

                Controls.Add(lbl);
                Controls.Add(ctrl);
                y += 36;
            }

            AddRow("Part Number",      txtPartNumber,    required: true);
            AddRow("Part Name",        txtPartName);
            AddRow("Customer Name",    txtCustomerName);
            AddRow("Customer Location",txtCustLocation,  required: true);
            AddRow("Quantity",         txtQuantity);
            AddRow("Operator Name",    txtOperatorName,  readOnly: true);
            AddRow("Inspector Name",   txtInspectorName, required: true);
            AddRow("Invoice No",       txtInvoiceNo,     required: true);
            AddRow("Hodek Part No",    txtHodekPartNo);

            // MFG Date (DateTimePicker — special control)
            var lblDate = new Label
            {
                Text      = "MFG Date",
                Location  = new Point(LBL_X, y + 4),
                Size      = new Size(LBL_W, 22),
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(50, 50, 70)
            };
            dtpMfgDate.Location = new Point(TXT_X, y);
            dtpMfgDate.Size     = new Size(TXT_W, 28);
            dtpMfgDate.Format   = DateTimePickerFormat.Short;
            dtpMfgDate.Value    = DateTime.Today;
            Controls.Add(lblDate);
            Controls.Add(dtpMfgDate);
            y += 36;

            // ── buttons ──────────────────────────────────────────────────
            y += 8;
            btnPrint.Text      = "🖨  Print Label";
            btnPrint.Location  = new Point(16, y);
            btnPrint.Size      = new Size(150, 38);
            btnPrint.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPrint.BackColor = Color.FromArgb(0, 120, 215);
            btnPrint.ForeColor = Color.White;
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Cursor    = Cursors.Hand;
            btnPrint.Click    += BtnPrint_Click;

            btnCancel.Text         = "Cancel";
            btnCancel.Location     = new Point(176, y);
            btnCancel.Size         = new Size(100, 38);
            btnCancel.Font         = new Font("Segoe UI", 9F);
            btnCancel.FlatStyle    = FlatStyle.Flat;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Cursor       = Cursors.Hand;

            Controls.Add(btnPrint);
            Controls.Add(btnCancel);
            AcceptButton = btnPrint;
            CancelButton = btnCancel;

            // ── placeholder texts ─────────────────────────────────────────
            txtPartNumber.PlaceholderText    = "e.g. PN-12345";
            txtCustLocation.PlaceholderText  = "e.g. Pune, MH";
            txtInspectorName.PlaceholderText = "Inspector full name";
            txtInvoiceNo.PlaceholderText     = "Invoice / challan number";
            txtHodekPartNo.PlaceholderText   = "Hodek drawing / part no";

            // ── form size ─────────────────────────────────────────────────
            ClientSize = new Size(500, y + 60);
        }

        // ── print handler ────────────────────────────────────────────────
        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPartNumber.Text))
            { Warn("Part Number is required."); txtPartNumber.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtCustLocation.Text))
            { Warn("Customer Location is required."); txtCustLocation.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtInspectorName.Text))
            { Warn("Inspector Name is required."); txtInspectorName.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
            { Warn("Invoice No is required."); txtInvoiceNo.Focus(); return; }

            try
            {
                // Strip // comments
                string prn = string.Join("\r\n",
                    _prnContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                               .Where(l => !l.TrimStart().StartsWith("//"))) + "\r\n";

                // Replace all tokens
                prn = prn
                    .Replace("{PartNumber}",      txtPartNumber.Text.Trim())
                    .Replace("{PartName}",        txtPartName.Text.Trim())
                    .Replace("{CustomerName}",    txtCustomerName.Text.Trim())
                    .Replace("{CustomerLocation}",txtCustLocation.Text.Trim())
                    .Replace("{Quantity}",        txtQuantity.Text.Trim())
                    .Replace("{QtyOrdered}",      txtQuantity.Text.Trim())
                    .Replace("{QtyScanned}",      txtQuantity.Text.Trim())
                    .Replace("{OperatorName}",    txtOperatorName.Text.Trim())
                    .Replace("{InspectorName}",   txtInspectorName.Text.Trim())
                    .Replace("{InvoiceNo}",       txtInvoiceNo.Text.Trim())
                    .Replace("{HodekPartNo}",     txtHodekPartNo.Text.Trim())
                    .Replace("{MFGDate}",         dtpMfgDate.Value.ToString("dd-MM-yyyy"))
                    .Replace("{OrderNo}",         _orderNo);

                string tempPrn = Path.Combine(_baseFolder, "customer_label.prn");
                File.WriteAllText(tempPrn, prn, Encoding.ASCII);

                var psi = new ProcessStartInfo
                {
                    FileName              = "cmd.exe",
                    Arguments             = $"/c copy /b \"{tempPrn}\" \"\\\\localhost\\{_printerShareName}\"",
                    UseShellExecute       = false,
                    CreateNoWindow        = true,
                    RedirectStandardOutput= true,
                    RedirectStandardError = true
                };
                Process.Start(psi)?.WaitForExit(3000);

                MessageBox.Show("Label sent to printer ✅", "Printed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Warn(string msg) =>
            MessageBox.Show(msg, "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
