using Application.Shared.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Products.Application.Domain.Identity;
using Products.Application.Interfaces.Service;

namespace Products.Application.Features.Identity.Roles.Queries
{
    public record GetRoleByIdQuery(string RoleId) : IRequest<GetRoleByIdResponse>;

    public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, GetRoleByIdResponse>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionsService _service;
        private readonly IMapper _mapper;

        public GetRoleByIdHandler(RoleManager<ApplicationRole> roleManager, IPermissionsService service, IMapper mapper)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetRoleByIdResponse> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
            {
                throw new NotFoundException($"Role with the Id : '{request.RoleId}' was not found.");
            }
            var result = _mapper.Map<GetRoleByIdResponse>(role);
            result.Modules = await _service.GetModulesWithPermissionsByRole(request.RoleId);
            return result;
        }
    }

    public class GetRoleByIdResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public List<ModuleWithPermissionsResponse> Modules { get; set; } = default!;
        public bool Active { get; set; }
    }

    public class GetRoleByIdMapProfile : Profile
    {
        public GetRoleByIdMapProfile()
        {
            CreateMap<ApplicationRole, GetRoleByIdResponse>();
        }
    }

}
