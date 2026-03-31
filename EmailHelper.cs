using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public static class EmailHelper
    {
        public static void SendEmailAsync(string filePath, string subject = "Shift Report")
        {
            Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(filePath)) return;

                    var (sender, password, receiver) = DatabaseHelper.GetEmailSettings();

                    if (string.IsNullOrWhiteSpace(sender) ||
                        string.IsNullOrWhiteSpace(password) ||
                        string.IsNullOrWhiteSpace(receiver))
                        return;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(sender);
                    mail.To.Add(receiver);
                    mail.Subject = subject;
                    mail.Body = $"Please find attached shift report.\nGenerated: {DateTime.Now}";

                    FileStream fs = new FileStream(
                        filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    mail.Attachments.Add(new Attachment(fs, "report.csv", "text/csv"));

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential(sender, password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    // Log silently — don't block UI
                    File.AppendAllText("error.log",
                        $"[{DateTime.Now}] Email Error: {ex.Message}\n");
                }
            });
        }
    }
}