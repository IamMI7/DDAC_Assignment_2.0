using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Models
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<IdeaVM> Ideas { get; set; }
        public DbSet<MaterialsVM> Materials { get; set; }
        public DbSet<SupportTicketVM> SupportTickets { get; set; }
    }
}
