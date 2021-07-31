using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Helper
{
    public class Utility
    {
        public static string admin = "Admin";
        public static string customer = "Customer";
        public static string staff = "Staff";

        public static List<SelectListItem> GetRolesDropDown(bool isAdmin)
        {
            if (isAdmin)
            {
                return new List<SelectListItem>
                {
                    new SelectListItem{Value=Utility.admin, Text=Utility.admin },
                    new SelectListItem{Value=Utility.staff, Text=Utility.staff },
                    new SelectListItem{Value=Utility.customer, Text=Utility.customer }
                };
            }
            else
            {
                return new List<SelectListItem>
                {
                    new SelectListItem{ Value=Utility.customer, Text=Utility.customer}
                };
            }
        }
    }
}
