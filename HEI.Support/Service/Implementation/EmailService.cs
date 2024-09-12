using System.Net.Mail;
using System.Net;
using HEI.Support.Service.Interface;
using HEI.Support.Areas.Admin.Models;

namespace HEI.Support.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly SMTPConfig m;
        public EmailService(SmtpClient smtpClient, SMTPConfig m)
        {
            _smtpClient = smtpClient;
            this.m = m;
        }

        public async Task<bool> SendMailAsync(string toAddress, string subject, string msg/*, SMTPConfig m*/)
        {
            bool success = false;

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(m.SmtpFromAddress, m.SmtpDisplayName);
                    mail.Subject = subject;
                    mail.Body = msg;
                    mail.IsBodyHtml = true;

                    foreach (string to in toAddress.Split(',').Where(t => !string.IsNullOrWhiteSpace(t)))
                    {
                        mail.To.Add(new MailAddress(to));
                    }

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    //using (SmtpClient smtp = new SmtpClient
                    //{
                    //    Host = m.SmtpHost,
                    //    Port = m.SmtpPort,
                    //    EnableSsl = m.SmtpSsl,
                    //    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //    UseDefaultCredentials = false,
                    //    Credentials = new NetworkCredential(m.SmtpUsername, m.SmtpPassword)
                    //})
                    //{
                    //    // SmtpClient.Send does not have an async version, so use Task.Run for a fire-and-forget operation
                    //    await Task.Run(() => smtp.Send(mail));
                    //    success = true;
                    //}
                    await _smtpClient.SendMailAsync(mail);
                    success = true;

                }
            }
            catch (Exception ex)
            {
                // Log exception details (consider using a logging library)
                Console.WriteLine($"Email sending error: {ex.Message}");
                // Log stack trace
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return success;
            }

            //logMessage(m.SmtpFromAddress, toAddress, subject, msg, success, userID, username);
            return success;
        }

        public async Task<bool> SendEmailToMultipleUsersAsync(IEnumerable<string> emails, string subject, string msg)
        {
            bool success = false;

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(m.SmtpFromAddress, m.SmtpDisplayName);
                    mail.Subject = subject;
                    mail.Body = msg;
                    mail.IsBodyHtml = true;

                    foreach (string to in emails)
                    {
                        mail.To.Add(new MailAddress(to));
                    }

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    await _smtpClient.SendMailAsync(mail);
                    success = true;

                }
            }
            catch (Exception ex)
            {
                // Log exception details (consider using a logging library)
                Console.WriteLine($"Email sending error: {ex.Message}");
                // Log stack trace
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return success;
            }

            return success;
        }
    }
}
