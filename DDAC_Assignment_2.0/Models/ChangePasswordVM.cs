using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Models
{
    public class ChangePasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters longs.", MinimumLength = 6)]
        public string newPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("newPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirmationPassword { get; set; }
    }
}
