using ECommerceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

public class Authorization
{
    // Main method to seed roles and admin user
    public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        // Create roles if they don't exist
        await CreateRolesAsync(roleManager);

        // Create admin user if not already created
        await CreateAdminUserAsync(userManager, roleManager);
    }

    // Method to create roles (Admin, User)
    private static async Task CreateRolesAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        var roles = new[] { "Admin", "User" };

        // Check if the roles exist, if not, create them
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
    }

    // Method to create the Admin user if not already created
    private static async Task CreateAdminUserAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        var adminEmail = "waqar.tanger12@gmail.com"; // Admin email
        var adminPassword = "Admin@123"; // Admin password

        // Check if admin user exists by email
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var admin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                IsActive = true,
                EmailConfirmed = true
            };

            // Create the admin user
            var createAdmin = await userManager.CreateAsync(admin, adminPassword);
            if (createAdmin.Succeeded)
            {
                // Assign the Admin role to the user
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
