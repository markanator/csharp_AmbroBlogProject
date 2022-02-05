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
                Email = "markanator13@hotmail.com",
                UserName = "markanator13@hotmail.com",
                FirstName = "Mark",
                LastName = "Ambro",
                DisplayName = "Mambroc",
                PhoneNumber = "800 555 1212",
                EmailConfirmed = true,
            };
            await _userManager.CreateAsync(adminUser, "Password123!");
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            var modUser = new BlogUser()
            {
                Email = "cory.kris73@ethereal.email",
                UserName = "cory.kris73@ethereal.email",
                FirstName = "cory",
                LastName = "kris73",
                DisplayName = "cory.kris73",
                PhoneNumber = "800 456 3214",
                EmailConfirmed = true,
            };
            await _userManager.CreateAsync(modUser, "Password123!");
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }
    }
}
