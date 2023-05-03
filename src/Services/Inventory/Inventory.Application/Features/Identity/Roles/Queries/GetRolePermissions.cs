using Inventory.Application.Interfaces.Service;
using MediatR;

namespace Inventory.Application.Features.Identity.Roles.Queries
{

    public record GetRolePermissionsQuery(string RoleId) : IRequest<List<ModuleWithPermissionsResponse>>;

    public class GetRolePermissions : IRequestHandler<GetRolePermissionsQuery, List<ModuleWithPermissionsResponse>>
    {

        private readonly IPermissionsService _service;

        public GetRolePermissions(IPermissionsService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<List<ModuleWithPermissionsResponse>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetModulesWithPermissionsByRole(request.RoleId);
        }
    }
}

