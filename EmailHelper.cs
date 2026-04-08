using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BarcodeBartenderApp
{
    public static class EmailHelper
    {
        public static void SendEmailAsync(string filePath, string subject = "Report")
            => SendEmailAsync(new List<string> { filePath }, subject);

        public static void SendEmailAsync(List<string> filePaths, string subject = "Report")
        {
            Task.Run(() =>
            {
                try
                {
                    var (sender, password, receiver) = DatabaseHelper.GetEmailSettings();
                    if (string.IsNullOrWhiteSpace(sender) ||
                        string.IsNullOrWhiteSpace(password) ||
                        string.IsNullOrWhiteSpace(receiver)) return;

                    var mail = new MailMessage();
                    mail.From = new MailAddress(sender);
                    mail.To.Add(receiver);
                    mail.Subject = subject;
                    mail.Body = $"Report attached.\nGenerated: {DateTime.Now:dd-MM-yyyy HH:mm:ss}";

                    var streams = new List<FileStream>();
                    foreach (var path in filePaths)
                    {
                        if (!File.Exists(path)) continue;
                        var fs = new FileStream(path, FileMode.Open,
                            FileAccess.Read, FileShare.ReadWrite);
                        streams.Add(fs);
                        string ext = Path.GetExtension(path).ToLower();
                        string mime = ext == ".pdf" ? "application/pdf" : "text/csv";
                        mail.Attachments.Add(new Attachment(fs, Path.GetFileName(path), mime));
                    }

                    var smtp = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(sender, password),
                        EnableSsl = true
                    };
                    smtp.Send(mail);

                    foreach (var fs in streams) fs.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText("error.log",
                        $"[{DateTime.Now}] Email Error: {ex.Message}\n");
                }
            });
        }
    }
}