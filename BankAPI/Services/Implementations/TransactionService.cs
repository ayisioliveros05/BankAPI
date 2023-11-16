using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using BankAPI.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace BankAPI.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private BankAPIDbContext _context;
        ILogger<TransactionService> _logger;
        private AppSettings _settings;
        private static string _settlementAccount;
        private readonly IAccountService _account;

        public TransactionService(BankAPIDbContext context, ILogger<TransactionService> logger, IOptions<AppSettings> settings, IAccountService account)
        {
            _context = context;
            _logger = logger;
            _settings = settings.Value;
            _settlementAccount = _settings.SettlementAccount;
            _account = account;
        }

        public Response MakeBalanceInquiry(string AccountNumber, string TransactionPin)
        {
            Response response = new Response();
            Models.Transaction transaction = new Models.Transaction();

            var authUser = _account.Authenticate(AccountNumber, TransactionPin);

            transaction.TransactionStatus = TranStatus.Success;
            transaction.TransactionType = TranType.BalanceInquiry;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TransactionDestinationAccount = "";
            transaction.TransactionAmount = authUser.CurrentAccountBalance;
            transaction.TransactionDate = DateTime.Now;

            response.ResponseCode = HttpStatusCode.OK;
            response.ResponseMessage = "Transaction successfully!";
            response.Data = JsonConvert.SerializeObject(transaction);

            // add balance inquiry to DB
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return response;
        }

        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Models.Transaction transaction = new Models.Transaction();

            var sourceAccount = _account.GetByAccountNumber(AccountNumber);

            //update account balance
            sourceAccount.CurrentAccountBalance -= Amount;

            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TransactionDestinationAccount = "";
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;

            // check if there is an update
            if ((_context.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
            {
                transaction.TransactionStatus = TranStatus.Success;

                response.ResponseCode = HttpStatusCode.OK;
                response.ResponseMessage = "Transaction successfully!";
                response.Data = JsonConvert.SerializeObject(transaction);
            }
            else
            {
                transaction.TransactionStatus = TranStatus.Failed;

                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Transaction failed!";
                response.Data = JsonConvert.SerializeObject(transaction);
            }

            // add withdrawal to DB
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return response;
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Models.Transaction transaction = new Models.Transaction();

            var sourceAccount = _account.GetByAccountNumber(_settlementAccount);
            var destinationAccount = _account.GetByAccountNumber(AccountNumber);

            //update account balance
            sourceAccount.CurrentAccountBalance -= Amount;
            destinationAccount.CurrentAccountBalance += Amount;

            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _settlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;

            // check if there is an update
            if ((_context.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                (_context.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
            {
                transaction.TransactionStatus = TranStatus.Success;

                response.ResponseCode = HttpStatusCode.OK;
                response.ResponseMessage = "Transaction successfully!";
                response.Data = JsonConvert.SerializeObject(transaction);
            }
            else
            {
                transaction.TransactionStatus = TranStatus.Failed;

                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Transaction failed!";
                response.Data = JsonConvert.SerializeObject(transaction);
            }

            // add deposit to DB
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return response;
        }

        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Models.Transaction transaction = new Models.Transaction();

            var sourceAccount = _account.GetByAccountNumber(FromAccount);
            var destinationAccount = _account.GetByAccountNumber(ToAccount);

            // update account balance
            sourceAccount.CurrentAccountBalance -= Amount;
            destinationAccount!.CurrentAccountBalance += Amount;

            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;

            // check if there is an update
            if ((_context.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                (_context.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
            {
                transaction.TransactionStatus = TranStatus.Success;

                response.ResponseCode = HttpStatusCode.OK;
                response.ResponseMessage = "Transaction successfully!";
                response.Data = JsonConvert.SerializeObject(transaction);
            }
            else
            {
                transaction.TransactionStatus = TranStatus.Failed;

                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Transaction failed!";
                response.Data = JsonConvert.SerializeObject(transaction);
            }

            // add fund transfer to DB
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return response;
        }
    }
}
