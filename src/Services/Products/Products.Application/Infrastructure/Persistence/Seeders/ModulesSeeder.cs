using Products.Application.Shared.Permissions;

namespace Products.Application.Infrastructure.Persistence.Seeders
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
