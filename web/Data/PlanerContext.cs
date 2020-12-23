using web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace web.Data
{
    public class PlanerContext : IdentityDbContext<ApplicationUser>
    {
        public PlanerContext(DbContextOptions<PlanerContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}