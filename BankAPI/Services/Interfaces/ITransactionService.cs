using BankAPI.Models;

namespace BankAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Response MakeBalanceInquiry(string AccountNumber, string TransactionPin);
        Response MakeWithdrawal(string AccountNumber, decimal Amount, string TrasactionPin);
        Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin);
        Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin);
    }
}
