namespace Products.Application.Domain.Identity
{
    public class RolePermission
    {
        public string Id { get; set; } = default!;
        public virtual string RoleId { get; set; } = default!;
        public virtual string PermissionId { get; set; } = default!;
        public virtual ApplicationRole Role { get; set; } = default!;
        public virtual Permission Permission { get; set; } = default!;
    }
}
