using AutoMapper;
using Azure;
using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using BankAPI.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private BankAPIDbContext _context;
        private ITransactionService _transaction;
        private IAccountService _account;
        IMapper _mapper;

        public TransactionsController(BankAPIDbContext context, ITransactionService transaction, IAccountService account, IMapper mapper)
        {
            _context = context;
            _transaction = transaction;
            _account = account;
            _mapper = mapper;
        }

        [EnableCors]
        [HttpPost]
        [Route("balanceInquiry")]
        public IActionResult MakeBalanceInquiry(string AccountNumber, string TransactionPin)
        {
            Models.Response response = new Models.Response();
            
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Account Number must be 10-digit.";

                return BadRequest(response);
            }

            var authUser = _account.Authenticate(AccountNumber, TransactionPin);

            if (authUser == null)
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Invalid credentials!";

                return BadRequest(response);
            }

            return Ok(_transaction.MakeBalanceInquiry(AccountNumber, TransactionPin));
        }

        [EnableCors]
        [HttpPost]
        [Route("withdrawal")]
        public IActionResult MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Models.Response response = new Models.Response();

            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Account Number must be 10-digit.";

                return BadRequest(response);
            }

            var authUser = _account.Authenticate(AccountNumber, TransactionPin);

            if (authUser == null)
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Invalid credentials!";

                return BadRequest(response);
            }

            if (authUser.CurrentAccountBalance < Amount)
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Insufficient balance.";

                return BadRequest(response);
            }

            var withdraw = _transaction.MakeWithdrawal(AccountNumber, Amount, TransactionPin);

            if (withdraw.ResponseCode == HttpStatusCode.BadRequest) 
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Transaction failed!";

                return BadRequest(response);
            }
            
            return Ok(withdraw);
        }

        [EnableCors]
        [HttpPost]
        [Route("deposit")]
        public IActionResult MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Models.Response response = new Models.Response();

            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Account Number must be 10-digit.";

                return BadRequest(response);
            }

            var authUser = _account.Authenticate(AccountNumber, TransactionPin);

            if (authUser == null)
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Invalid credentials!";

                return BadRequest(response);
            }

            var deposit = _transaction.MakeDeposit(AccountNumber, Amount, TransactionPin);

            if (deposit.ResponseCode == HttpStatusCode.BadRequest)
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Transaction failed!";

                return BadRequest(response);
            }

            return Ok(deposit);
        }

        [EnableCors]
        [HttpPost]
        [Route("transfer")]
        public IActionResult MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            Models.Response response = new Models.Response();

            if (!Regex.IsMatch(FromAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$") || !Regex.IsMatch(ToAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Account Number must be 10-digit.";

                return BadRequest(response);
            }

            var authUser = _account.Authenticate(FromAccount, TransactionPin);

            if (authUser == null)
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Invalid credentials!";

                return BadRequest(response);
            }

            if (authUser.CurrentAccountBalance < Amount)
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Insufficient balance.";

                return BadRequest(response);
            }
            
            if (!_context.Accounts.Any(x => x.AccountNumberGenerated == ToAccount))
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Invalid Recipient Account Number.";

                return BadRequest(response);
            }

            var transfer = _transaction.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin);

            if (transfer.ResponseCode == HttpStatusCode.BadRequest)
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "Transaction failed!";

                return BadRequest(response);
            }

            return Ok(transfer);
        }
    }
}
