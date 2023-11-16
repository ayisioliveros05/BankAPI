using AutoMapper;
using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using BankAPI.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private BankAPIDbContext _context;
        private IAccountService _account;

        IMapper _mapper;

        public AccountsController(BankAPIDbContext context, IAccountService account, IMapper mapper)
        {
            _context = context;
            _account = account;
            _mapper = mapper;
        }

        // register new account
        [EnableCors]
        [HttpPost]
        [Route("register")]
        public IActionResult RegisterNewAccount([FromBody] RegisterModel newAccount)
        {
            if (!ModelState.IsValid) return BadRequest(newAccount);

            Models.Response response = new Models.Response();

            if (_context.Accounts.Any(x => x.Email == newAccount.Email))
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "An account already exists with this email.";

                return BadRequest(response);
            }

            if (_context.Accounts.Any(x => x.PhoneNumber == newAccount.PhoneNumber))
            {
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.ResponseMessage = "An account already exists with this phone number.";

                return BadRequest(response);
            }

            // map to account
            var account = _mapper.Map<Account>(newAccount);
            
            return Ok(_account.Create(account, newAccount.Pin, newAccount.ConfirmPin));
        }
    }
}
