namespace Inventory.Application.Domain.Identity
{
    public class Permission
    {
        public string Id { get; set; } = default!;
        public virtual string ModuleId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Order { get; set; } = 0;
        public bool Required { get; set; } = false;
        public virtual Module Module { get; set; } = default!;
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = default!;
    }
}
