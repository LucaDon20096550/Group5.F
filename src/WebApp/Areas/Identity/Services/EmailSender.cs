using System.Net.Mail;
using FluentEmail.Smtp;
using FluentEmail.Core;
using System.Threading.Tasks;
using System.Text;
using FluentEmail.Razor;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebApp.Areas.Identity.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(){
            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 25
                //DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                //PickupDirectoryLocation = @"C:\Demos"
            });

            Email.DefaultSender = sender;
        }

        public async Task SendEmailAsync2(string email, string subject, string message)
        {
            var mail = await Email
                .From("info@zmdh.nl")
                .To(email)
                .Subject(subject)
                .Body(message)
                .SendAsync();
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            Email.DefaultRenderer = new RazorRenderer();
            
            StringBuilder template = new();
            template.AppendLine("Dear user,");
            template.AppendLine(message);
            template.AppendLine("- The ZMDH Team");

            var mail = await Email
                .From("info@zmdh.nl")
                .To(email)
                .Subject(subject)
                .UsingTemplate(template.ToString(), new { FirstName = "ZMDH", ProductName = "Client" })
                .SendAsync();
        }
    }
}