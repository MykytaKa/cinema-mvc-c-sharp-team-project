using Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _sendGridApiKey;

        public EmailService(IConfiguration configuration)
        {
            _sendGridApiKey = configuration["SendGrid:ApiKey"]; 
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("no-reply@yourdomain.com", "Cinema App");
            var recipient = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, recipient, subject, body, body);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }
    }
}
