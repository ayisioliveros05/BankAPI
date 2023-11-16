using BankAPI.Models;

namespace BankAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Account Create(Account account, string Pin, string ConfirmPin);
        Account Authenticate(string AccountNumber, string Pin);
        Account GetByAccountNumber(string AccountNumber);
    }
}
