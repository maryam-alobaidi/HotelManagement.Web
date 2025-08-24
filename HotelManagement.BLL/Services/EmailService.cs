using HotelManagement.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace HotelManagement.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public void SendEmail(string to, string subject, string body)
        {
            var smtpConfig = _config.GetSection("Smtp");

            using (var client = new SmtpClient(smtpConfig["Host"], int.Parse(smtpConfig["Port"])))
            {
                client.Credentials = new NetworkCredential(smtpConfig["UserName"], smtpConfig["Password"]);
                client.EnableSsl = bool.Parse(smtpConfig["EnableSSL"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpConfig["UserName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);
                client.Send(mailMessage);
            }
        }
    }
}
