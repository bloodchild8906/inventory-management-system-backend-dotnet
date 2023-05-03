using Inventory.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Inventory.Application.Infrastructure.Authentication
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor contextAccessor)
        {
            UserId = contextAccessor?.HttpContext?.User.FindFirstValue("uid");
        }

        public string? UserId { get; } = default!;
    }
}
