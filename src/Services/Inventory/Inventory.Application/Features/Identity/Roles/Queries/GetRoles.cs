using Application.Shared.Extensions;
using Application.Shared.Models;
using AutoMapper;
using FluentValidation;
using Inventory.Application.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Features.Identity.Roles.Queries
{
    public class GetRolesQuery : IRequest<PagedList<GetRolesResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; } = default!;
        public string? SortOrder { get; set; } = default!;
        public string? Name { get; set; }
        public bool? Active { get; set; }
    }

    public class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
    {
        public GetRolesQueryValidator()
        {
            RuleFor(r => r.OrderBy)
                .Must(r => MustBeNullOrExistsInSet(r));

            RuleFor(r => r.PageNumber)
                .GreaterThan(0);

            RuleFor(r => r.PageSize)
                .GreaterThan(0);
        }

        public bool MustBeNullOrExistsInSet(string? sortBy)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return true;
            }
            var set = new HashSet<string>
            {
                "id",
                "name",
                "active"
            };
            return set.Contains(sortBy.ToLower());
        }
    }

    public class GetRolesHandler : IRequestHandler<GetRolesQuery, PagedList<GetRolesResponse>>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public GetRolesHandler(RoleManager<ApplicationRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private Func<IQueryable<ApplicationRole>, IOrderedQueryable<ApplicationRole>> GetOrderByField(string orderBy, bool ascending)
        {
            switch (orderBy)
            {
                case "id":
                    return ascending ? q => q.OrderBy(r => r.Id) : q => q.OrderByDescending(r => r.Id);
                case "name":
                    return ascending ? q => q.OrderBy(r => r.Name) : q => q.OrderByDescending(r => r.Name);
                case "active":
                    return ascending ? q => q.OrderBy(r => r.Active) : q => q.OrderByDescending(r => r.Active);
                default:
                    return ascending ? q => q.OrderBy(r => r.Name) : q => q.OrderByDescending(r => r.Name);
            }
        }

        public async Task<PagedList<GetRolesResponse>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var ascending = request.SortOrder == "desc" ? false : true;
            var orderBy = request.OrderBy == null ? "" : request.OrderBy;

            var baseQuery = _roleManager.Roles;
            var orderByField = GetOrderByField(orderBy, ascending);
            baseQuery = orderByField(baseQuery);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                baseQuery = baseQuery.Where(u => EF.Functions.Like(u.Name, $"%{request.Name}%"));
            }

            if (request.Active != null)
            {
                baseQuery = baseQuery.Where(u => u.Active == request.Active);
            }

            var roles = await baseQuery.ToPagedListAsync(request.PageNumber, request.PageSize);

            var result = _mapper.Map<PagedList<GetRolesResponse>>(roles);
            return result;
        }
    }
    public class GetRolesResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool Active { get; set; }
    }

    public class GetRolesMapProfile : Profile
    {
        public GetRolesMapProfile()
        {
            CreateMap<ApplicationRole, GetRolesResponse>();
            CreateMap<PagedList<ApplicationRole>, PagedList<GetRolesResponse>>();
        }
    }
}
