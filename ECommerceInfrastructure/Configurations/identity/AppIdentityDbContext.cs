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
            builder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    img = "https://t3.ftcdn.net/jpg/02/43/12/34/360_F_243123463_zTooub557xEWABDLk0jJklDyLSGl2jrr.jpg",
                    UserName = "احمد ناصر",
                    Email = "john.doe@example.com",
                    PhoneNumber = "059213417",
                    address = "نابلس-شارع الاقصى-حي",
                    IsActive = true,
                    createdAt = DateTime.Parse("2024-11-01T10:00:00Z")
                },
                new User
                {
                    Id = 2,
                    img = "https://png.pngtree.com/png-vector/20230918/ourmid/pngtree-man-in-shirt-smiles-and-gives-thumbs-up-to-show-approval-png-image_10094381.png",
                    UserName = "محمد جابر",
                    Email = "jane.smith@example.com",
                    PhoneNumber = "059213417",
                    address = "طولكرم-شارع الاقصى-حي",
                    IsActive = false,
                    createdAt = DateTime.Parse("2024-11-05T14:30:00Z")
                },
                new User
                {
                    Id = 3,
                    img = "https://img.freepik.com/free-photo/lifestyle-people-emotions-casual-concept-confident-nice-smiling-asian-woman-cross-arms-chest-confident-ready-help-listening-coworkers-taking-part-conversation_1258-59335.jpg",
                    UserName = "ديمه ذنيبي",
                    Email = "michael.johnson@example.com",
                    PhoneNumber = "059213417",
                    address = "نابلس-شارع الاقصى-حي",
                    IsActive = true,
                    createdAt = DateTime.Parse("2024-11-10T08:45:00Z")
                },
                new User
                {
                    Id = 4,
                    img = "https://t4.ftcdn.net/jpg/03/83/25/83/360_F_383258331_D8imaEMl8Q3lf7EKU2Pi78Cn0R7KkW9o.jpg",
                    UserName = "وقار طنجير",
                    Email = "emily.brown@example.com",
                    PhoneNumber = "059213417",
                    address = "خليل-شارع الاقصى-حي",
                    IsActive = true,
                    createdAt = DateTime.Parse("2024-01-01T19:23:45Z")
                },
                new User
                {
                    Id = 5,
                    img = "https://static.vecteezy.com/system/resources/previews/024/354/252/non_2x/businessman-isolated-illustration-ai-generative-free-photo.jpg",
                    UserName = "سعيد محي",
                    Email = "daniel.garcia@example.com",
                    PhoneNumber = "059213417",
                    address = "نابلس-شارع الاقصى-حي",
                    IsActive = false,
                    createdAt = DateTime.Parse("2024-11-15T12:00:00Z")
                }
            );
        }
    }
}
