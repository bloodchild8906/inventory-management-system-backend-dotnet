namespace Products.Application.Interfaces.Service
{
    public class PermissionResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool Required { get; set; } = default;
        public bool Selected { get; set; } = default;
        public int Order { get; set; } = 0;
    }

    public class ModuleWithPermissionsResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Order { get; set; }
        public List<PermissionResponse> Permissions { get; set; } = default!;
    }

    public interface IPermissionsService
    {
        Task<List<ModuleWithPermissionsResponse>> GetModulesWithPermissions();
        Task<List<ModuleWithPermissionsResponse>> GetModulesWithPermissionsByRole(string roleId);
    }
}
