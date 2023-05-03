using Microsoft.AspNetCore.Identity;

namespace Inventory.Application.Domain.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; } = default!;
        public bool Active { get; set; } = true;
        public virtual ICollection<RolePermission> Permissions { get; set; } = default!;
    }
}
