using Inventory.Application.Domain.Identity;
using Inventory.Application.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Application.Infrastructure.Persistence.Seeders
{
    public static class UsersSeeder
    {
        public static async Task CreateUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.Users.Any())
                return;

            var user = new ApplicationUser
            {
                RoleId = UserConstants.AdminRoleId,
                Email = UserConstants.AdminEmail,
                UserName = UserConstants.AdminEmail,
                Fullname = "Admin",
                EmailConfirmed = true,
                Active = true
            };

            await userManager.CreateAsync(user, UserConstants.AdminPassword);
        }
    }
}
