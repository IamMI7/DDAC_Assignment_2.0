using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Models
{
    public class SupportTicketVM
    {
        [Key]
        public string ID { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 to 30 characters long.")]
        public string name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "This is not a valid email address.")]
        public string email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Subject must be between 3 to 30 characters long.")]
        public string subject { get; set; }

        [Required]
        [StringLength(600, MinimumLength = 10, ErrorMessage = "Message must be between 10 to 600 characters long.")]
        public string message { get; set; }
    }
}
