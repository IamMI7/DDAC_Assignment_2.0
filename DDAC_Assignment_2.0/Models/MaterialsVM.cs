using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Models
{
    public class MaterialsVM
    {
        [Key]
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Please Select a Material")]
        public string Material { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Please Select a Material")]
        public string ImageName { get; set; }
    }
}
