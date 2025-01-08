using ECommerceCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceInfrastructure.Configurations.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart> Carts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<CartItem>().ToTable("CartItems");
            // Define the unique index on the Name property For Category
            modelBuilder.Entity<Category>()
                .HasIndex(p => p.Name)
                .IsUnique();


            // Define the unique index on the Name property For Products
            modelBuilder.Entity<Color>()
                .HasIndex(p => p.Name)
                .IsUnique();
        

        modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.AdditionalImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductColor>()
                .HasKey(pc => new { pc.ProductId, pc.ColorId });

            

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Colors)
                .WithOne(pc => pc.Product)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Color>()
                .HasMany(c => c.ProductColors)
                .WithOne(pc => pc.Color)
                .HasForeignKey(pc => pc.ColorId)
                .OnDelete(DeleteBehavior.Cascade);

           /* modelBuilder.Entity<User>()
               .HasOne(u => u.Cart)
               .WithOne(c => c.User)
               .HasForeignKey<Cart>(c => c.UserId);*/

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItem)
                .HasForeignKey(ci => ci.ProductId);

        }

    }
}
