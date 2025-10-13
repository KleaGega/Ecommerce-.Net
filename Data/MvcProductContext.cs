using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using MVCProject.Models;
using MVCProject.Models;

namespace MVCProject.Data
{
    public class MvcProductContext : IdentityDbContext<Users>
    {
        public MvcProductContext(DbContextOptions<MvcProductContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }
    }
}
