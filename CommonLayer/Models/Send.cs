﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace CommonLayer.Models
{
    public class Send
    {
        public string SendMail(string ToEmail, string Token)
        {
            string FromEmail = "shankar89479@gmail.com";
            MailMessage message = new MailMessage(FromEmail, ToEmail);
            string MailBody = "The token for the reset password: " + Token;
            message.Subject = "Token generated for resetting password";
            message.Body = MailBody.ToString();
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            NetworkCredential credential = new NetworkCredential("shankar89479@gmail.com", "lwuf deyi wpwj aylr");

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = credential;
            smtpClient.Send(message);

            return ToEmail;
        }
    }
}
