using Microsoft.AspNetCore.Identity;
using Products.Application.Domain.Identity;
using Products.Application.Shared.Constants;
using Products.Application.Shared.Permissions;

namespace Products.Application.Infrastructure.Persistence.Seeders
{
    public static class RolesSeeder
    {
        public static async Task CreateRoles(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            if (roleManager.Roles.Any())
                return;

            var adminRole = new ApplicationRole
            {
                Id = UserConstants.AdminRoleId,
                Name = "Admin",
                Description = "Admin Role",
                Active = true
            };

            await roleManager.CreateAsync(adminRole);

            foreach (var module in PermissionsFactory.CreateModulesWithPermissions())
            {
                foreach (var permission in module.Permissions)
                {
                    var rolePermission = new RolePermission { PermissionId = permission.Id , RoleId = adminRole.Id};
                    context.RolePermissions.Add(rolePermission);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
