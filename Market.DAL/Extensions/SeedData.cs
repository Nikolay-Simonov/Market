using System;
using System.Linq;
using System.Text;
using Market.DAL.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Market.DAL.Extensions
{
    public static class SeedData
    {
        /// <summary>
        /// Создает записи об администраторе и ролях если их не существует.
        /// </summary>
        /// <param name="app"></param>
        /// <exception cref="ArgumentNullException">The argument "<paramref name="app"/>" was null.</exception>
        public static async void EnsureAdmin(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            var usersManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var rolesManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
            var configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();

            if (!await rolesManager.Roles.Select(r => r.Name).ContainsAsync("Admin"))
            {
                await rolesManager.CreateAsync(new ApplicationRole("Admin"));
            }

            if (!await rolesManager.Roles.Select(r => r.Name).ContainsAsync("ContentManager"))
            {
                await rolesManager.CreateAsync(new ApplicationRole("ContentManager"));
            }

            if (!await rolesManager.Roles.Select(r => r.Name).ContainsAsync("User"))
            {
                await rolesManager.CreateAsync(new ApplicationRole("User"));
            }

            bool adminExists = (await usersManager.GetUsersInRoleAsync("Admin")).Any();

            if (adminExists)
            {
                return;
            }

            var admin = new ApplicationUser
            {
                FirstName = "Admin",
                LastName = "Adminov",
                UserName = configuration.GetValue<string>("AdminSettings:Email"),
                Email = configuration.GetValue<string>("AdminSettings:Email"),
                Address = "localhost",
                EmailConfirmed = true
            };

            var stringBuilder = new StringBuilder();

            IdentityResult createResult = await usersManager
                .CreateAsync(admin,configuration.GetValue<string>("AdminSettings:Password"));

            if (createResult.Succeeded)
            {
                IdentityResult addRolesResult = await usersManager
                    .AddToRolesAsync(admin, new[] { "Admin", "ContentManager" });

                if (createResult.Succeeded)
                {
                    return;
                }

                foreach (IdentityError error in addRolesResult.Errors)
                {
                    stringBuilder.AppendLine(error.Description);
                }
            }
            else
            {
                foreach (IdentityError error in createResult.Errors)
                {
                    stringBuilder.AppendLine(error.Description);
                }
            }

            string errorMessage = stringBuilder.ToString();

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new Exception(errorMessage);
            }
        }
    }
}