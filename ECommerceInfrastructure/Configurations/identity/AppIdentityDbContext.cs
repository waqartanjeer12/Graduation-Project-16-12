using ECommerceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceInfrastructure.Configurations.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppIdentityDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seeding data for the User entity
            
        }
    }
}
