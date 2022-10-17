using AutoMapper;
using BankAPI.Dto_s;
using BankAPI.Models;
using BankAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        private readonly IMapper _mapper;

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("create_new_transaction")]
        public IActionResult CreateTransaction([FromBody] CreateNewTransactionDTO newTransaction)
        {
            if (!ModelState.IsValid) return BadRequest(newTransaction);

            var transaction = _mapper.Map<Transaction>(newTransaction);
            return Ok(_transactionService.CreateNewTransaction(transaction));
        }

        [HttpGet]
        [Route("get_transaction_by_accNumber")]
        public IActionResult GetByAccNumber(string accountNumber, DateTime date, string pin)
        {
            var trans = _transactionService.FindTransactionByAccountNumber(accountNumber,date, pin);
            var dtoTrans = _mapper.Map<IList<GetTransactionDTO>>(trans);
            return Ok(dtoTrans);
        }

        [HttpPost]
        [Route("make_deposit")]
        public IActionResult MakeDeposit(string accountNumber, decimal amount, string transactionPin)
        {
            if (!Regex.IsMatch(accountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account number should be 10 digits");
            return Ok(_transactionService.MakeDeposit(accountNumber, amount, transactionPin));
        }

        [HttpPost]
        [Route("make_withdrawal")]
        public IActionResult MakeWithdrawal(string accountNumber, decimal amount, string transactionPin)
        {
            if (!Regex.IsMatch(accountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account number should be 10 digits");
            return Ok(_transactionService.MakeWithdraw(accountNumber, amount, transactionPin));
        }

        [HttpPost]
        [Route("make_transfer")]
        public IActionResult MakeTransfer(string fromAccount, string toAccount, decimal amount, string transactionPin)
        {
            if (!Regex.IsMatch(fromAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$") || !Regex.IsMatch(toAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("Account number should be 10 digits");
            return Ok(_transactionService.MakeTransfer(fromAccount, toAccount, amount, transactionPin));
        }
    }
}
