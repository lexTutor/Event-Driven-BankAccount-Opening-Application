using BankAccount.Shared.Domain;

namespace BankAccount.Shared.Contracts
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
