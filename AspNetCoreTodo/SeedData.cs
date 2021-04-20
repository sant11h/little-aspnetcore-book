﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await EnsureRolesAsync(roleManager);

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            await EnsureTestAdminAsync(userManager);
        }

        private static async Task EnsureTestAdminAsync(UserManager<IdentityUser> userManager)
        {
            var testAdmin = await userManager.Users
                .Where(x => x.UserName == "admin@todo.local")
                .SingleOrDefaultAsync();

            if (testAdmin != null)
            {
                return;
            }

            testAdmin = new IdentityUser
            {
                UserName = "admin@todo.local",
                Email = "admin@todo.local",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(testAdmin, "NotSecure123!");
            await userManager.AddToRoleAsync(testAdmin, Constants.AdministratorRole);
        }

        private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var alreadyExists = await roleManager.RoleExistsAsync(Constants.AdministratorRole);
            if (alreadyExists)
            {
                return;
            }

            await roleManager.CreateAsync(new IdentityRole(Constants.AdministratorRole));
        }
    }
}
