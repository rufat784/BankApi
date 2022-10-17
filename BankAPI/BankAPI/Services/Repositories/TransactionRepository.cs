using BankAPI.Models;
using BankAPI.Units;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Services.Repositories
{
    public class TransactionRepository : ITransactionService
    {
        private readonly AppDbContext _context;
        ILogger<TransactionRepository> _logger;
        private AppSettings _settings;
        private static string _ourBankSettlementAccount;
        private readonly IAccountService _accountService;

        public TransactionRepository(AppDbContext context, ILogger<TransactionRepository> logger, IOptions<AppSettings> settings, IAccountService accountService)
        {
            _context = context;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementAccount = _settings.OurBankSettlementAccount;
            _accountService = accountService;
        }
        public Response CreateNewTransaction(Transaction transaction)
        {
            Response response = new Response();
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Success";
            response.Data = null;

            return response;
        }

        public IEnumerable<Transaction> FindTransactionByAccountNumber(string accNumber, DateTime date, string transactionPin)
        {
            var auth = _accountService.Auth(accNumber, transactionPin);
            if (auth == null) throw new ApplicationException("Invalid credentials");

            return _context.Transactions.Where(x => (x.TransactionSourceAccount == accNumber)&& (x.TransactionDate == date)).ToList(); 
        }

        public Response MakeDeposit(string accountNumber, decimal amount, string transactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            var auth = _accountService.Auth(accountNumber, transactionPin);
            if (auth == null) throw new ApplicationException("Invalid credentials");

            try
            {
                sourceAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);
                destinationAccount = _accountService.GetByAccountNumber(accountNumber);

                sourceAccount.CurrentBalance -= amount;
                destinationAccount.CurrentBalance += amount;

                if ((_context.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_context.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Success";
                    response.Data = null;
                }
                else
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Failed";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _ourBankSettlementAccount;
            transaction.TransactionDestinationAccount = accountNumber;
            transaction.TransactionAmount = amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"Transaction from => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)}" +
                $" to => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                $"on date => {transaction.TransactionDate} " +
                $"for amount => {JsonConvert.SerializeObject(transaction.TransactionAmount)} " +
                $"transaction type => {transaction.TransactionType} " +
                $"transaction status => {transaction.TransactionStatus} ";

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return response;
        }

        public Response MakeTransfer(string fromAccount, string toAccount, decimal amount, string transactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            var auth = _accountService.Auth(fromAccount, transactionPin);
            if (auth == null) throw new ApplicationException("Invalid credentials");

            try
            {
                sourceAccount = _accountService.GetByAccountNumber(fromAccount);
                destinationAccount = _accountService.GetByAccountNumber(toAccount);

                sourceAccount.CurrentBalance -= amount;
                destinationAccount.CurrentBalance += amount;

                if ((_context.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_context.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Success";
                    response.Data = null;
                }
                else
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Failed";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = fromAccount;
            transaction.TransactionDestinationAccount = toAccount;
            transaction.TransactionAmount = amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"Transaction from => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)}" +
                $" to => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                $"on date => {transaction.TransactionDate} " +
                $"for amount => {JsonConvert.SerializeObject(transaction.TransactionAmount)} " +
                $"transaction type => {transaction.TransactionType} " + 
                $"transaction status => {transaction.TransactionStatus} ";

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return response;
        }

        public Response MakeWithdraw(string accountNumber, decimal amount, string transactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            var auth = _accountService.Auth(accountNumber, transactionPin);
            if (auth == null) throw new ApplicationException("Invalid credentials");

            try
            {
                sourceAccount = _accountService.GetByAccountNumber(accountNumber);
                destinationAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);

                sourceAccount.CurrentBalance -= amount;
                //destinationAccount.CurrentBalance += amount;

                if ((_context.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) ||
                    (_context.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Success";
                    response.Data = null;
                }
                else
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Failed";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = accountNumber;
            transaction.TransactionDestinationAccount = _ourBankSettlementAccount;
            transaction.TransactionAmount = amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"Transaction from => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)}" +
                $" to => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                $"on date => {transaction.TransactionDate} " +
                $"for amount => {JsonConvert.SerializeObject(transaction.TransactionAmount)} " +
                $"transaction type => {transaction.TransactionType} " +
                $"transaction status => {transaction.TransactionStatus} ";

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return response;
        }
    }
}
