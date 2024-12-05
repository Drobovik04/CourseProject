using CourseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Database
{
    public static class DatabaseInitializer
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "admin@example.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var userName = "test";

            if (await userManager.FindByNameAsync(userName) == null)
            {
                var newUser = new IdentityUser
                {
                    UserName = userName,
                    Email = "test@example.com",
                };
                var result = await userManager.CreateAsync(newUser, "test");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "User");
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
