using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string roleName { get; set; }
    }
}
