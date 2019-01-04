using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Recollectable.Core.Shared.Enums;
using Recollectable.Infrastructure.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public EmailService(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public async Task Send(string recipient, string subject, string message, MailType type = MailType.Welcome)
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

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message, Encoding.UTF8, MediaTypeNames.Text.Html);

                //TODO When Published on Web Server, use _env.WebRootPath + Add real links in EmailLayout
                LinkedResource bannerResource = new LinkedResource(Path.Combine(_env.ContentRootPath, @"Assets\Banner.png"), "image/png")
                {
                    ContentId = "Banner"
                };
                LinkedResource facebookResource = new LinkedResource(Path.Combine(_env.ContentRootPath, @"Assets\Facebook.png"), "image/png")
                {
                    ContentId = "Facebook"
                };
                LinkedResource twitterResource = new LinkedResource(Path.Combine(_env.ContentRootPath, @"Assets\Twitter.png"), "image/png")
                {
                    ContentId = "Twitter"
                };
                LinkedResource linkedInResource = new LinkedResource(Path.Combine(_env.ContentRootPath, @"Assets\LinkedIn.png"), "image/png")
                {
                    ContentId = "LinkedIn"
                };
                LinkedResource followResource = new LinkedResource(Path.Combine(_env.ContentRootPath, @"Assets\Follow.png"), "image/png")
                {
                    ContentId = "Follow"
                };

                bannerResource.ContentType.Name = "recollectable.png";
                twitterResource.ContentType.Name = "twitter.png";

                htmlView.LinkedResources.Add(bannerResource);
                htmlView.LinkedResources.Add(facebookResource);
                htmlView.LinkedResources.Add(twitterResource);
                htmlView.LinkedResources.Add(linkedInResource);
                htmlView.LinkedResources.Add(followResource);

                switch (type)
                {
                    case MailType.Confirmation:
                        LinkedResource activateResource = new LinkedResource(Path.Combine(_env.ContentRootPath, @"Assets\Activate.png"), "image/png")
                        {
                            ContentId = "Activate"
                        };
                        htmlView.LinkedResources.Add(activateResource);
                        break;
                    case MailType.Reset:
                        LinkedResource resetResource = new LinkedResource(Path.Combine(_env.ContentRootPath, @"Assets\Reset.png"), "image/png")
                        {
                            ContentId = "Reset"
                        };
                        htmlView.LinkedResources.Add(resetResource);
                        break;
                    case MailType.Welcome:
                        LinkedResource welcomeResource = new LinkedResource(Path.Combine(_env.ContentRootPath, @"Assets\Welcome.png"), "image/png")
                        {
                            ContentId = "Welcome"
                        };
                        htmlView.LinkedResources.Add(welcomeResource);
                        break;
                }

                using (var email = new MailMessage())
                {
                    email.To.Add(new MailAddress(recipient));
                    email.From = new MailAddress(_configuration["Email:Email"]);
                    email.AlternateViews.Add(htmlView);
                    email.Subject = subject;
                    email.Body = message;
                    email.IsBodyHtml = true;
                    client.Send(email);
                }

                await Task.CompletedTask;
            }
        }
    }
}