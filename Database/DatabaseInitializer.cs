using CourseProject.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Database
{
    public static class DatabaseInitializer
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
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
                var adminUser = new AppUser
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
                var newUser = new AppUser
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

            var userName1 = "test1";

            if (await userManager.FindByNameAsync(userName1) == null)
            {
                var newUser = new AppUser
                {
                    UserName = userName1,
                    Email = "test1@example.com",
                };
                var result = await userManager.CreateAsync(newUser, "test1");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "User");
                }
            }

            if (!context.Topics.Any())
            {
                context.Topics.AddRange(
                    new Topic { Name = "Education" },
                    new Topic { Name = "Health" },
                    new Topic { Name = "Technology" },
                    new Topic { Name = "Science" },
                    new Topic { Name = "Quiz" },
                    new Topic { Name = "Other" }
                );

                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();
        }
    }
}
