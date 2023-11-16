using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using System.Text;

namespace BankAPI.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private BankAPIDbContext _context;

        public AccountService(BankAPIDbContext context)
        {
            _context = context;
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            // validate pin
            if (!Pin.Equals(ConfirmPin))
                throw new ArgumentException("Pins do not match", "Pin");

            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);
            var now = DateTime.Now;
            
            account.PinHash = pinHash;
            account.PinSalt = pinSalt;
            account.AccountName = String.Concat(account.FirstName, ' ', account.LastName);
            account.DateCreated = now;
            account.DateLastUpdated = now;

            // add new account to DB
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return account;
        }

        public Account Authenticate(string AccountNumber, string Pin)
        {
            // check if account is exist
            var account = _context.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null)
                return null;

            // verify pinHash
            if (!VerifyPinHash(Pin, account.PinHash, account.PinSalt))
                return null;

            return account;
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _context.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            if (account == null)
                return null;

            return account;
        }

        #region Methods

        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin))
                throw new ArgumentNullException("Pin");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i])
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}
