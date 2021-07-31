using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Models
{
    public class UserVM
    {
        [Key]
        [Required]
        public string email { get; set; }

        public string id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string contactNumber { get; set; }

        [Required]
        public string roleName { get; set; }
    }
}
