using BankAccount.Shared.Contracts;
using BankAccount.Shared.Domain;
using BankAccount.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.WorkFlowServices
{
    public class CommunicateWithMemberWorkflowService : IWorkflowService
    {
        private readonly ILogger<CommunicateWithMemberWorkflowService> _logger;
        private readonly IMailService _mailService;

        public CommunicateWithMemberWorkflowService(
            ILogger<CommunicateWithMemberWorkflowService> logger,
            IMailService mailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        public WorkFlow WorkFlow => WorkFlow.CommunicateWithMember;

        public OperationResult<dynamic> ValidateMetadata(string metadata)
        {
            try
            {
                CommunicateWithMemberPayload model = JsonConvert.DeserializeObject<CommunicateWithMemberPayload>(metadata);

                if (string.IsNullOrWhiteSpace(model.AccountNumber))
                    return OperationResult<dynamic>.Failed($"{nameof(model.AccountNumber)} is required");

                if (string.IsNullOrWhiteSpace(model.Email))
                    return OperationResult<dynamic>.Failed($"{nameof(model.Email)} is required");

                if (string.IsNullOrWhiteSpace(model.FullName))
                    return OperationResult<dynamic>.Failed($"{nameof(model.FullName)} is required");

                return new OperationResult<dynamic>
                {
                    Result = model,
                    Successful = true
                };
            }
            catch (JsonException ex)
            {
                _logger.LogWarning($"Unable to deserialize metadata errors:{ex.Message}");
                return OperationResult<dynamic>.Failed("Unable to deserialize metadata");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"An error occured during data processing {ex}");
                throw;
            }
        }

        public async Task ExecuteAsync(string metadata, string sessionId)
        {
            try
            {
                var validateMetdata = ValidateMetadata(metadata);
                if (!validateMetdata.Successful)
                {
                    _logger.LogWarning($"Metadata validation was unsuccessful with error: {validateMetdata.Result}");
                    return;
                }

                CommunicateWithMemberPayload model = (CommunicateWithMemberPayload)validateMetdata.Result;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles/CommunicateWithMember.html");
                if (!File.Exists(filePath))
                    throw new ArgumentException("CommunicateWithMember.html file not found");

                var fileText = await File.ReadAllTextAsync(filePath);
                var updatedFile = fileText.Replace("**username**", model.FullName).Replace("**account**", model.AccountNumber);
                var mailRequest = new MailRequest
                {
                    ToEmail = model.Email,
                    Subject = "Your Bank Account has been Created",
                    Body = updatedFile
                };

                _logger.LogDebug("Initiating Call to Mail Service");

                await _mailService.SendEmailAsync(mailRequest);

                _logger.LogDebug("Email sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
    }
}
