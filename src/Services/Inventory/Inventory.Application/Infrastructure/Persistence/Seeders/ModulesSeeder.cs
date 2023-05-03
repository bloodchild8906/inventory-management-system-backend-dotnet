using Inventory.Application.Infrastructure.Persistence;
using Inventory.Application.Shared.Permissions;

namespace Inventory.Application.Infrastructure.Persistence.Seeders
{
    public class ModulesSeeder
    {
        public static async Task CreateModulesWithPermissions(ApplicationDbContext context)
        {
            if (context.Modules.Any())
            {
                return;
            }
            var modulesWithPermissions = PermissionsFactory.CreateModulesWithPermissions();
            context.AddRange(modulesWithPermissions);
            await context.SaveChangesAsync();
        }
    }
}
