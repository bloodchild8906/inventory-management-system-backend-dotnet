using Microsoft.AspNetCore.Identity;

namespace Inventory.Application.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string RoleId { get; set; } = default!;
        public string Fullname { get; set; } = default!;
        public bool Active { get; set; }
    }
}
