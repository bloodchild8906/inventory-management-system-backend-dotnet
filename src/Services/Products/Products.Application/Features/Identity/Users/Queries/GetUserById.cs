using Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Products.Application.Domain.Identity;
using Products.Application.Features.Identity.Roles.Queries;
using Products.Application.Interfaces.Service;

namespace Products.Application.Features.Identity.Users.Queries
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionsService _service;

        public GetUserByIdHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IPermissionsService service)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException($"The User with the Id {request.UserId} was not found.");
            }

            var role = await _roleManager.FindByIdAsync(user.RoleId);


            var modules = await _service.GetModulesWithPermissionsByRole(role.Id);


            return new GetUserByIdResponse
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Email = user.Email,
                Active = user.Active,
                Role = new GetRoleByIdResponse
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    Active = role.Active,
                    Modules = modules
                }
            };
        }
    }

    public class GetUserByIdQuery : IRequest<GetUserByIdResponse>
    {
        public string UserId { get; set; } = default!;
    }

    public class GetUserByIdResponse
    {
        public string Id { get; set; } = default!;
        public GetRoleByIdResponse Role { get; set; } = default!;
        public string Fullname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool Active { get; set; }
    }
}
