using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Cryptography.X509Certificates;

namespace NovaBlog.Services.Interfaces
{
    public interface IBPEmailService 
    {
        public Task SendEmailAsync(string email,string subject, string htmlMessage);
    }
}
