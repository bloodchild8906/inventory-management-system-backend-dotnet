using Application.Shared.Extensions;
using Application.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Products.Application.Domain.Identity;
using Products.Application.Interfaces.Authentication;
using System.Text.Json.Serialization;

namespace Products.Application.Features.Identity.Users.Queries
{
    // https://www.davepaquette.com/archive/2019/01/28/paging-large-result-sets-with-dapper-and-sql-server.aspx
    public class GetAllUsersQuery : IRequest<PagedList<GetAllUsersResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; } = default!;
        public string? SortOrder { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? RoleId { get; set; } = default;
        public bool? Active { get; set; }
    }

    public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
    {
        public GetAllUsersQueryValidator() 
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
                "fullname",
                "role",
                "active"
            };
            return set.Contains(sortBy.ToLower());
        }
    }

    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, PagedList<GetAllUsersResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public GetAllUsersHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IAuthenticatedUserService authenticatedUserService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _authenticatedUserService = authenticatedUserService ?? throw new ArgumentNullException(nameof(authenticatedUserService));
        }

        private Func<IQueryable<GetAllUsersResponse>, IOrderedQueryable<GetAllUsersResponse>> GetOrderByField(string oderBy, bool ascending)
        {
            switch(oderBy)
            {
                case "id":
                    return ascending ? q => q.OrderBy(u => u.Id) : q => q.OrderByDescending(u => u.Id);
                case "name":
                    return ascending ? q => q.OrderBy(u => u.Fullname) : q => q.OrderByDescending(u => u.Fullname);
                case "role":
                    return ascending ? q => q.OrderBy(u => u.Role) : q => q.OrderByDescending(u => u.Role);
                case "active":
                    return ascending ? q => q.OrderBy(u => u.Active) : q => q.OrderByDescending(u => u.Active);
                default:
                    return ascending ? q => q.OrderBy(u => u.Fullname) : q => q.OrderByDescending(u => u.Fullname);
            }
        }


        public async Task<PagedList<GetAllUsersResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var ascending = request.SortOrder == "desc" ? false : true;
            var oderBy = request.OrderBy == null ? "" : request.OrderBy;

            var baseQuery  = from u in _userManager.Users
                                join r in _roleManager.Roles
                                on u.RoleId equals r.Id
                                select new GetAllUsersResponse
                                {
                                    Id = u.Id,
                                    Fullname = u.Fullname,
                                    RoleId = r.Id,
                                    Role = r.Name,
                                    Active = u.Active
                                };
            
            var orderByField = GetOrderByField(oderBy, ascending);
            baseQuery = orderByField(baseQuery);

            if (!string.IsNullOrWhiteSpace(_authenticatedUserService.UserId))
            {
                baseQuery = baseQuery.Where(u => u.Id != _authenticatedUserService.UserId);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                baseQuery = baseQuery.Where(u => EF.Functions.Like(u.Fullname, $"%{request.Name}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.RoleId))
            {
                baseQuery = baseQuery.Where(u => u.RoleId == request.RoleId);
            }


            if (request.Active != null)
            {
                baseQuery = baseQuery.Where(u => u.Active == request.Active);
            }

            return await baseQuery.ToPagedListAsync(request.PageNumber, request.PageSize);
        }
    }

    public class GetAllUsersResponse
    {
        public string Id { get; set; } = default!;
        public string Fullname { get; set; } = default!;
        [JsonIgnore]
        public string RoleId { get; set; } = default!;
        public string Role { get; set; } = default!;
        public bool Active { get; set; } = default!;
    }

}


