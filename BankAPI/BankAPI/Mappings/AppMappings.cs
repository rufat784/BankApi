using AutoMapper;
using BankAPI.Dto_s;
using BankAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Mappings
{
    public class AppMappings : Profile
    {
        public AppMappings()
        {
            CreateMap<RegisterNewAccountDTO, Account>();
            CreateMap<UpdateAccountDTO, Account>();
            CreateMap<Account, GetAccountDTO>();
            CreateMap<Transaction, GetTransactionDTO>();
            CreateMap<CreateNewTransactionDTO, Transaction>();

        }
    }
}
