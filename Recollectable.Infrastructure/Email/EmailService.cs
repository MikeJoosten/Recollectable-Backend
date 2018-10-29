using Microsoft.Extensions.Configuration;
using Recollectable.Core.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Send(string recipient, string subject, string message)
        {
            using (var client = new SmtpClient())
            {
                var credentials = new NetworkCredential
                {
                    UserName = _configuration["Email:Email"],
                    Password = _configuration["Email:Password"]
                };

                client.Credentials = credentials;
                client.Host = _configuration["Email:Host"];
                client.Port = Convert.ToInt32(_configuration["Email:Port"]);
                client.EnableSsl = true;

                using (var email = new MailMessage())
                {
                    email.To.Add(new MailAddress(recipient));
                    email.From = new MailAddress(_configuration["Email:Email"]);
                    email.Subject = subject;
                    email.Body = message;
                    client.Send(email);
                }

                await Task.CompletedTask;
            }
        }
    }
}