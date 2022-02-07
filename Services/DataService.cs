/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
using AmbroBlogProject.Data;
using AmbroBlogProject.Enums;
using AmbroBlogProject.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AmbroBlogProject.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            // create DB if not present
            await _dbContext.Database.MigrateAsync();
        // 1. seed a few roles into the system
            await SeedRolesAsync();
        // 2. seed a user into the system
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (_dbContext.Roles.Any())
            {
                // DB already seeded
                return;
            }

            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            if (_dbContext.Users.Any()) { return; }

            // s1: create new instance of user
            var adminUser = new BlogUser()
            {
                Email = "AdminUser@example.com",
                UserName = "AdminUser@example.com",
                FirstName = "Admin",
                LastName = "User",
                DisplayName = "Admin User",
                PhoneNumber = "800 555 1212",
                EmailConfirmed = true,
            };
            await _userManager.CreateAsync(adminUser, "Password123!");
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            var modUser = new BlogUser()
            {
                Email = "ModUser@example.com",
                UserName = "ModUser@example.com",
                FirstName = "Moderator",
                LastName = "User",
                DisplayName = "Mod User",
                PhoneNumber = "800 456 3214",
                EmailConfirmed = true,
            };
            await _userManager.CreateAsync(modUser, "Password123!");
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }
    }
}
