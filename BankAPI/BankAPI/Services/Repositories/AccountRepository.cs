using BankAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAPI.Services.Repositories
{
    public class AccountRepository : IAccountService
    {
        private readonly AppDbContext _context;
        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }
        public Account Auth(string accountNumber, string pin)
        {
            var account = _context.Accounts.Where(x => x.AccNumbGenerated == accountNumber).SingleOrDefault();

            if (account == null)
                return null;

            if (!VerifyPinHash(pin, account.PinHash, account.PinSalt))
                return null;

            return account;
        }

        private static bool VerifyPinHash(string pin, byte[] pinHash, byte[] pinnSalt)
        {
            if (string.IsNullOrWhiteSpace(pin)) throw new ArgumentNullException("Pin");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(pinnSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }

        public Account Create(Account account, string pin, string confirmPin)
        {
            if (_context.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("Account already exist");

            if (!pin.Equals(confirmPin)) throw new ApplicationException("Pins doesn't match");

            byte[] pinHash, pinSalt;
            CreatePinHash(pin, out pinHash, out pinSalt);

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            _context.Accounts.Add(account);
            _context.SaveChanges();

            return account;
        }

        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) 
            {
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
                pinSalt = hmac.Key;
            }
        }

        public void Delete(int id)
        {
            var account = _context.Accounts.Find(id);

            if (account != null) 
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Account> GetAccounts()
        {
           return _context.Accounts.ToList();
        }

        public Account GetByAccountNumber(string accountNumber)
        {
            var account = _context.Accounts.Where(x => x.AccNumbGenerated == accountNumber).FirstOrDefault();
            if (account == null) return null;

            return account;
        }

        public Account GetById(int id)
        {
            var account = _context.Accounts.Where(x => x.Id == id).FirstOrDefault();
            if (account == null) return null;

            return account;
        }

        public void Update(Account account, string pin = null)
        {
            var accountToBeUpdated = _context.Accounts.Where(x => x.Id == account.Id).SingleOrDefault();
            if (accountToBeUpdated == null) throw new ApplicationException("Account doesn't exists");

            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                if (_context.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("This Email " + account.Email + " already exists");

                accountToBeUpdated.Email = account.Email;
            }

            if (!string.IsNullOrWhiteSpace(account.Phone))
            {
                if (_context.Accounts.Any(x => x.Phone == account.Phone)) throw new ApplicationException("This Phone Number " + account.Phone + " already exists");

                accountToBeUpdated.Phone = account.Phone;
            }

            if (!string.IsNullOrWhiteSpace(pin))
            {
                byte[] pinHash, pinSalt;
                CreatePinHash(pin, out pinHash, out pinSalt);

                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;
            }

            accountToBeUpdated.UpdateDate = DateTime.Now;
            _context.Accounts.Update(accountToBeUpdated);
            _context.SaveChanges();
        }
    }
}
