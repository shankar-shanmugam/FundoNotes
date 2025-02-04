using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace CommonLayer.Models
{
    public class EmailService
    {
        private readonly string _fromEmail;
        private readonly string _password;

        public EmailService(string fromEmail, string password)
        {
            _fromEmail = fromEmail;
            _password = password;
        }
        public string SendCollaborationEmail(string toEmail, string noteTitle, string ownerName)
        {
            try
            {
                MailMessage message = new MailMessage(_fromEmail, toEmail);
                string mailBody = $@"
                    <html>
                    <body>
                        <h2>FundooNotes Collaboration Invitation</h2>
                        <p>Hello,</p>
                        <p>{ownerName} has shared a note titled '{noteTitle}' with you.</p>
                        <p>You can now view and edit this note in your FundooNotes account.</p>
                        <p>Best regards,<br/>FundooNotes Team</p>
                    </body>
                    </html>";

                message.Subject = $"Note Shared: {noteTitle}";
                message.Body = mailBody;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                NetworkCredential credential = new NetworkCredential(_fromEmail, _password);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = credential;
                smtpClient.Send(message);

                return toEmail;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send collaboration email: {ex.Message}");
            }
        }
    }
}
