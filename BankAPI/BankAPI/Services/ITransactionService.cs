using BankAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Services
{
    public interface ITransactionService
    {
        Response CreateNewTransaction(Transaction transaction);
        IEnumerable<Transaction> FindTransactionByAccountNumber(string accNumber, DateTime date,  string transactionPin);
        Response MakeDeposit(string accountNumber, decimal amount, string transactionPin);
        Response MakeWithdraw(string accountNumber, decimal amount, string transactionPin);
        Response MakeTransfer(string fromAccount, string toAccount, decimal amount, string transactionPin);

    }
}
