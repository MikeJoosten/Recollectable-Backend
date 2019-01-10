using Microsoft.Extensions.Options;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Enums;
using Recollectable.Infrastructure.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _configuration;

        public EmailService(IOptions<EmailConfiguration> options)
        {
            _configuration = options.Value;
        }

        // Use a CDN, like CloudFlare, to link images in Razorview externally
        public async Task SendAsync(string email, string subject, string message, MailType type = MailType.Welcome)
        {
            var client = new SendGridClient(_configuration.SendGridKey);

            var from = new EmailAddress("noreply@recollectable.com", "Recollectable");
            var to = new EmailAddress(email);
            var mail = MailHelper.CreateSingleEmail(from, to, subject, null, message);

            var response = await client.SendEmailAsync(mail);
        }
    }
}