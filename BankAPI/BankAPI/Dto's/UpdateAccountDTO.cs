using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Dto_s
{
    public class UpdateAccountDTO
    {
        [Key]
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin can not be more than 4 digits")]
        public string Pin { get; set; }

        [Required]
        [Compare("Pin", ErrorMessage = "Pin doesn't match")]
        public string ConfirmPin { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
