using BusinessLogicLayer.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class MailtrapService : IEmailService
    {
        private readonly IConfiguration _config;
        public MailtrapService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["MailTrapTesting:EmailFrom"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["MailTrapTesting:Host"], Convert.ToInt16(_config["MailTrapTesting:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["MailTrapTesting:Username"], _config["MailTrapTesting:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
