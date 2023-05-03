using Inventory.Application.Infrastructure.Persistence;
using Inventory.Application.Interfaces.Service;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Infrastructure.Service
{
    public class PermissionsService : IPermissionsService
    {
        private readonly ApplicationDbContext _context;

        public PermissionsService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<ModuleWithPermissionsResponse>> GetModulesWithPermissions()
        {
            var modulesQuery = from m in _context.Modules
                               orderby m.Order
                               select new ModuleWithPermissionsResponse
                               {
                                   Id = m.Id,
                                   Name = m.Name,
                                   Order = m.Order,
                                   Permissions = (from p in _context.Permissions
                                                  where p.ModuleId == m.Id
                                                  orderby p.Order
                                                  select new PermissionResponse
                                                  {
                                                      Id = p.Id,
                                                      Name = p.Name,
                                                      Order = p.Order,
                                                      Required = p.Required,
                                                      Selected = false
                                                  }).ToList()
                               };

            return await modulesQuery.ToListAsync();
        }

        public async Task<List<ModuleWithPermissionsResponse>> GetModulesWithPermissionsByRole(string roleId)
        {
            var modules = await GetModulesWithPermissions();
            var permissionsList = await _context.RolePermissions.Where(r => r.RoleId == roleId).Select(s => s.PermissionId).ToListAsync();
            for (int moduleIndex = 0; moduleIndex < modules.Count; moduleIndex++)
            {
                var module = modules[moduleIndex];
                for (int permissionIndex = 0; permissionIndex < module.Permissions.Count; permissionIndex++)
                {
                    var permission = module.Permissions[permissionIndex];
                    permission.Selected = permissionsList.Contains(permission.Id);
                }
            }
            return modules;
        }
    }
}
