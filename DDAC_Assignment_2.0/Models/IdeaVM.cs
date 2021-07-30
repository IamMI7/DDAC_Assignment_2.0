using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Models
{
    public class IdeaVM
    {
        [Key]
        public Int32 IdeaID { get; set; }

        public string Image { get; set; }

        [StringLength(50, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 50 characters")]
        public string Title { get; set; }

        public string Curator { get; set; }

        public DateTime DatePublish { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Please Select a Material")]
        public string Material { get; set; }

        //[NotMapped]
        //public List<SelectListItem> MaterialList { get; } = new List<SelectListItem>
        //{
        //    new SelectListItem { Value = "Cardboard", Text = "Cardboard" },
        //    new SelectListItem { Value = "Glass", Text = "Glass" },
        //    new SelectListItem { Value = "Plastic", Text = "Plastic" },
        //    new SelectListItem { Value = "Cans", Text = "Metal/Tin Cans" },
        //    new SelectListItem { Value = "Wood", Text = "Wood" },
        //    new SelectListItem { Value = "Paper", Text = "Paper" },
        //    new SelectListItem { Value = "Clothing", Text = "Clothing" },
        //    new SelectListItem { Value = "Furniture", Text = "Furniture" },
        //    new SelectListItem { Value = "Other", Text = "Other" }
        //};
    }
}
