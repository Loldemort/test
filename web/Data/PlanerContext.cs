using web.Models;
using Microsoft.EntityFrameworkCore;

namespace web.Data
{
    public class PlanerContext : DbContext //
    {
        public PlanerContext(DbContextOptions<PlanerContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Friends> Friends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}