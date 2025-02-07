using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string _sendGridApiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailService(IConfiguration configuration)
        {
            _sendGridApiKey = configuration["SendGrid:ApiKey"]; 
            _fromEmail = configuration["SendGrid:FromEmail"];
            _fromName = configuration["SendGrid:FromName"];
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var fromAddress = new EmailAddress(_fromEmail, _fromName);
            var recipient = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(fromAddress, recipient, subject, body, body);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }
    }
}
