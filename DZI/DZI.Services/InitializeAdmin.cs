using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CampRating.Services
{
    public static class InitializeAdmin
    {
        public static async Task InitializeAdminAsync(IServiceProvider services)
        {
            // Create a scope from the IServiceProvider passed in
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            var adminUserName = "admin@admin.com";

            // Check if an administrator user already exists
            var existingAdmin = await userManager.FindByNameAsync(adminUserName);

            if (existingAdmin != null)
            {
                // Reset the administrator's password
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(existingAdmin);
                var passwordResult = await userManager.ResetPasswordAsync(existingAdmin, resetToken, "SecurePassword123!");

                if (!passwordResult.Succeeded)
                {
                    throw new Exception($"Password reset failed: {string.Join(", ", passwordResult.Errors)}");
                }

                // Save the updated administrator details
                var updateResult = await userManager.UpdateAsync(existingAdmin);
                if (!updateResult.Succeeded)
                {
                    throw new Exception($"Admin update failed: {string.Join(", ", updateResult.Errors)}");
                }
            }
            else
            {
                var adminUser = new User
                {
                    UserName = adminUserName,
                    FirstName = "Admin",
                    LastName = "User"
                };

                var createResult = await userManager.CreateAsync(adminUser, "SecurePassword123!");
                if (!createResult.Succeeded)
                {
                    throw new Exception($"Admin creation failed: {string.Join(", ", createResult.Errors)}");
                }

                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }

            var admin = await userManager.FindByNameAsync(adminUserName);
            if (!await userManager.IsInRoleAsync(admin, "Administrator"))
            {
                await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }
    }
}
