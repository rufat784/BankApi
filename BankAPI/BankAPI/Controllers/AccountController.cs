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
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register_new_account")]
        public IActionResult RegisterAccount([FromBody] RegisterNewAccountDTO newAccount)
        {
            if (!ModelState.IsValid) return BadRequest(newAccount);

            var account = _mapper.Map<Account>(newAccount);
            return Ok(_accountService.Create(account, newAccount.Pin, newAccount.ConfirmPin));
        }

        [HttpGet]
        [Route("get_all_accounts")]
        public IActionResult GetAccounts()
        {
            var accounts = _accountService.GetAccounts();
            var dtoAccounts = _mapper.Map<IList<GetAccountDTO>>(accounts);
            return Ok(dtoAccounts);
        }

        [HttpPost]
        [Route("authenticate")]
        public IActionResult Auth([FromBody] Authenticate model)
        {
            if (!ModelState.IsValid) return BadRequest(model);

            return Ok(_accountService.Auth(model.AccountNumber, model.Pin));
        }

        [HttpGet]
        [Route("get_by_acc_number")]
        public IActionResult GetByAccountNumber(string accountNumber)
        {
            if (!Regex.IsMatch(accountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account number should be 10 digits");

            var account = _accountService.GetByAccountNumber(accountNumber);
            var dtoAccNumb = _mapper.Map<GetAccountDTO>(account);
            return Ok(dtoAccNumb);
        }

        [HttpGet]
        [Route("get_acc_by_Id")]
        public IActionResult GetById(int id)
        {
            var account = _accountService.GetById(id);
            var dtoAccId = _mapper.Map<GetAccountDTO>(account);
            return Ok(dtoAccId);
        }

        [HttpPut]
        [Route("update_account")]
        public IActionResult UpdateAccount([FromBody] UpdateAccountDTO updateAccount)
        {
            if (!ModelState.IsValid) return BadRequest(updateAccount);

            var account = _mapper.Map<Account>(updateAccount);

            _accountService.Update(account, updateAccount.Pin);
            return Ok();
        }
    }
}
