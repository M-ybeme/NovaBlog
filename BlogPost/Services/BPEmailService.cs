using NovaBlog.Models;
using NovaBlog.Services.Interfaces;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

namespace NovaBlog.Services
{
    public class BPEmailService : IBPEmailService
    {

        private readonly MailSettings? _mailSettings;
        private readonly UserManager<BlogUser> _userManager;
        public BPEmailService(IOptions<MailSettings> mailSettings, UserManager<BlogUser> userManager)
        {
            _mailSettings = mailSettings.Value;
            _userManager = userManager;
        }
        
        public async Task SendEmailAsync(string email,string subject, string htmlMessage)
        {
            var emailSender = _mailSettings!.Email ?? Environment.GetEnvironmentVariable("Email");

            MimeMessage newEmail = new();

            newEmail.Sender = MailboxAddress.Parse(emailSender);

            var adminEmail = "MarloMayberry.90@gmail.com";
            newEmail.To.Add(MailboxAddress.Parse(adminEmail));

            

            newEmail.Subject = subject;
            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();
            

            using SmtpClient smtpClient = new();
            try
            {
                var host = _mailSettings.Host ?? Environment.GetEnvironmentVariable("Email");
                var port = _mailSettings.Port != 0 ? _mailSettings.Port : int.Parse(Environment.GetEnvironmentVariable("Port")!);
                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, _mailSettings.Password ?? Environment.GetEnvironmentVariable("Email"));

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }            
        }
    }
}
