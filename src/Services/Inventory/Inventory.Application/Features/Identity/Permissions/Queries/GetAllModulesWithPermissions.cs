using Inventory.Application.Interfaces.Service;
using MediatR;

namespace Inventory.Application.Features.Identity.Permissions.Queries
{
    public class GetAllModulesWithPermissionsQuery : IRequest<List<ModuleWithPermissionsResponse>> { }
    public class GetAllModulesWithPermissions : IRequestHandler<GetAllModulesWithPermissionsQuery, List<ModuleWithPermissionsResponse>>
    {
        private readonly IPermissionsService _service;

        public GetAllModulesWithPermissions(IPermissionsService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<List<ModuleWithPermissionsResponse>> Handle(GetAllModulesWithPermissionsQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetModulesWithPermissions();
        }
    }
}
