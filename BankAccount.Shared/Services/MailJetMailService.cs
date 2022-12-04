using BankAccount.Shared.Contracts;
using BankAccount.Shared.Domain;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankAccount.Shared.Services
{
    public class MailJetMailService : IMailService
    {
        private readonly MailjetClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailJetMailService> _logger;

        public MailJetMailService(
            MailjetClient client,
            IConfiguration configuration,
            ILogger<MailJetMailService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var emailBuilder = new TransactionalEmailBuilder()
                  .WithFrom(new SendContact(_configuration["MailJetSettings:Email"]))
                  .WithSubject(mailRequest.Subject)
                  .WithHtmlPart(mailRequest.Body)
                  .WithTo(new SendContact(mailRequest.ToEmail));

                if (mailRequest.Copies != null && mailRequest.Copies.Any())
                    mailRequest.Copies.ForEach(x => emailBuilder.WithCc(new SendContact(x)));

                var email = emailBuilder.Build();

                await _client.SendTransactionalEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message, ex);
                throw;
            }
        }
    }

}
