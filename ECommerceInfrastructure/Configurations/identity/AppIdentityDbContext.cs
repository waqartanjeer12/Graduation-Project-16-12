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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductColor>()
                .HasKey(pc => new { pc.ProductId, pc.ColorId });

        }
    }
}
