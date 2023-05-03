using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Products.Application.Domain.Identity;
using Products.Application.Infrastructure.Persistence;
using System.Security.Claims;

namespace Products.Application.Infrastructure.Authentication.Requirements
{
    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public PermissionRequirementHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || context.User?.FindFirstValue("uid") == null)
            {
                return;
            }

            var userId = context.User?.FindFirstValue("uid");
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) 
            {
                return;
            }

            var role = await _roleManager.FindByIdAsync(user.RoleId);

            if (role == null)
            {
                return;
            }

            var permissions = _context.RolePermissions.Where(x => x.PermissionId == requirement.Permission);


            if (permissions.Any())
            {
                context.Succeed(requirement);
            }

        }
    }
}
