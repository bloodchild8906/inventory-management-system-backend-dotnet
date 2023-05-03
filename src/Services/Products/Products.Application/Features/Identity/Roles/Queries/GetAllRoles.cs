using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Products.Application.Domain.Identity;

namespace Products.Application.Features.Identity.Roles.Queries
{
    public class GetAllRolesQuery : IRequest<List<GetAllRolesResponse>> 
    {
        public bool? Active { get; set; }
        public string? Name { get; set; }
    }

    public class GetAllRolesHandler : IRequestHandler<GetAllRolesQuery, List<GetAllRolesResponse>>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public GetAllRolesHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<GetAllRolesResponse>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _roleManager.Roles;
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                baseQuery = baseQuery.Where(r => EF.Functions.Like(r.Name, $"%{request.Name}%"));
            }

            if (request.Active != null)
            {
                baseQuery = baseQuery.Where(r => r.Active == request.Active);
            }
            var roles = await baseQuery.OrderBy(r => r.Name).ToListAsync();
            return _mapper.Map<List<GetAllRolesResponse>>(roles);
        }
    }

    public class GetAllRolesResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool Active { get; set; }
    }

    public class GetAllRolesMapProfile : Profile
    {
        public GetAllRolesMapProfile()
        {
            CreateMap<ApplicationRole, GetAllRolesResponse>();
        }
    }
}
