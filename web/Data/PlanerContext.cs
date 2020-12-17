using web.Models;
using Microsoft.EntityFrameworkCore;

namespace web.Data
{
    public class PlanerContext : DbContext
    {
        public PlanerContext(DbContextOptions<PlanerContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
    }
}