using ECommerceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceInfrastructure.Configurations.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Remove the unique index on UserName
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.NormalizedUserName).IsUnique(false);
            });

            // Unique index on Name property for Category
            modelBuilder.Entity<Category>()
                .HasIndex(p => p.Name)
                .IsUnique();

            // Unique index on Name property for Colors
            modelBuilder.Entity<Color>()
                .HasIndex(p => p.Name)
                .IsUnique();

            // Configure the Product-Category one-to-many relationship.
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Product-ProductImage one-to-many relationship.
            modelBuilder.Entity<Product>()
                .HasMany(p => p.AdditionalImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the composite key for ProductColor.
            modelBuilder.Entity<ProductColor>()
                .HasKey(pc => new { pc.ProductId, pc.ColorId });

            // Configure the Product-ProductColor one-to-many relationship.
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Colors)
                .WithOne(pc => pc.Product)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the Color-ProductColor one-to-many relationship.
            modelBuilder.Entity<Color>()
                .HasMany(c => c.ProductColors)
                .WithOne(pc => pc.Color)
                .HasForeignKey(pc => pc.ColorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the User-Cart one-to-one relationship.
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            // Configure the Cart-CartItem one-to-many relationship.
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the CartItem-Product many-to-one relationship.
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItem)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the User-Order one-to-many relationship.
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Order-CartItem one-to-many relationship.
            modelBuilder.Entity<Order>()
                .HasMany(o => o.CartItems)
                .WithOne(ci => ci.Order)
                .HasForeignKey(ci => ci.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Order-OrderItem one-to-many relationship.
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the Product-OrderItem one-to-many relationship.
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItem)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}