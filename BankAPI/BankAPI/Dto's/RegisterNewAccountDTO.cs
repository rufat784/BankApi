using BankAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Dto_s
{
    public class RegisterNewAccountDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string AccountName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        //public decimal CurrentBalance { get; set; }
        public AccountType AccountType { get; set; }
        //public string AccNumbGenerated { get; set; }
        //public byte[] PinHash { get; set; }
        //public byte[] PinSalt { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage ="Pin can not be more than 4 digits")]
        public string Pin { get; set; }

        [Required]
        [Compare("Pin", ErrorMessage ="Pin doesn't match")]
        public string ConfirmPin { get; set; }

    }
}
