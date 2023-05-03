using Application.Shared.Exceptions;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Products.Application.Domain.Identity;
using Products.Application.Infrastructure.Persistence;

namespace Products.Application.Features.Identity.Roles.Commands
{
    public record CreateRoleModule(string Id, List<string> PermissionsIds);
    public record CreateRoleCommand(string Name, string Description, List<CreateRoleModule> Modules) : IRequest<string>;
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        private readonly ApplicationDbContext _context;
        public CreateRoleCommandValidator(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage("'Name' is required.");

            RuleFor(r => r.Description)
                .NotEmpty()
                .NotNull()
                .WithMessage("'Description' is required.");

            RuleFor(r => r.Modules)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("'Modules are required.'")
                .NotEmpty().WithMessage("'Modules' are required.")
                .MustAsync(async (modules, cancellation) => await AllModulesExists(modules)).WithMessage("Some modules in the field 'Modules' does not exists.")
                .Must(m => AllModulesAreUnique(m)).WithMessage("The 'Modules' field can't contain duplicates.")
                .MustAsync(async (modules, cancellation) => await AllPermissionsExists(modules)).WithMessage("Some Permissions in the field 'Modules' does not exists.")
                .Must(m => AllPermissionsAreUnique(m)).WithMessage("The 'Modules' field can't contain duplicate permissions.")
                .Must(m => AtLeastOneModuleSelected(m)).WithMessage("The 'Modules' field must have at least one Module selected.")
                .MustAsync(async (modules, cancellation) => await AreSelectedModulesValid(modules)).WithMessage("The 'Modules' field have missing required permissions");
        }

        private bool AllModulesAreUnique(List<CreateRoleModule> modules)
        {
            var modulesIds = modules.Select(s => s.Id).ToList();
            return modules.Count == modulesIds.Distinct().Count();
        }

        private bool AllPermissionsAreUnique(List<CreateRoleModule> modules)
        {
            var permissionsCount = modules.Sum(m => m.PermissionsIds.Count);
            var permissions = modules.SelectMany(m => m.PermissionsIds).ToList();
            return permissionsCount == permissions.Distinct().Count();
        }


        private async Task<bool> AllModulesExists(List<CreateRoleModule> modules)
        {
            foreach (var m in modules)
            {
                var exists = await _context.Modules.AnyAsync(x => x.Id == m.Id);
                if (!exists)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> AllPermissionsExists(List<CreateRoleModule> modules)
        {
            foreach (var m in modules)
            {
                foreach (var p in m.PermissionsIds)
                {
                    var exists = await _context.Permissions.AnyAsync(x => x.Id == p);
                    if (!exists)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool AtLeastOneModuleSelected(List<CreateRoleModule> modules)
        {
            return modules.Any(m => m.PermissionsIds != null && m.PermissionsIds.Count > 0);
        }

        private async Task<bool> AreRequiredPermissionsSelected(CreateRoleModule module)
        {
            var requiredPermissions = await _context
                .Permissions
                .Where(p => p.ModuleId == module.Id && p.Required)
                .Select(p => p.Id)
                .ToListAsync();

            if (requiredPermissions.Count == 0)
            {
                return true;
            }

            var permissionsCount = 0;

            foreach (var permission in module.PermissionsIds)
            {
                if (requiredPermissions.Contains(permission))
                {
                    permissionsCount++;
                }
            }
            return permissionsCount == requiredPermissions.Count;
        }


        private async Task<int> GetNumberOfValidModules(List<CreateRoleModule> modules)
        {
            var count = 0;
            foreach (var m in modules)
            {
                if (await AreRequiredPermissionsSelected(m))
                {
                    count++;
                }
            }
            return count;
        }
        
        private async Task<bool> AreSelectedModulesValid(List<CreateRoleModule> modules)
        {
            var selectedModules = modules.Where(m => m.PermissionsIds != null && m.PermissionsIds.Count > 0).ToList();
            var numberOfValidModules = await GetNumberOfValidModules(modules);
            return numberOfValidModules == selectedModules.Count;
        }

    }

    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, string>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        public CreateRoleHandler(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, IMapper mapper)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<string> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var newRole = _mapper.Map<ApplicationRole>(request);
            var roleWithSameName = await _roleManager.FindByNameAsync(request.Name);

            if (roleWithSameName != null)
            {
                throw new DomainException($"The role with the Name {request.Name} already exists.");
            }

            await _roleManager.CreateAsync(newRole);
            var permissions = request.Modules.SelectMany(m => m.PermissionsIds).ToList();

            foreach (var permission in permissions)
            {
                var rolePermission = new RolePermission { PermissionId = permission, Role = newRole };
                _context.RolePermissions.Add(rolePermission);
            }

            await _context.SaveChangesAsync();

            return newRole.Id;
        }
    }

    public class CreateRoleMapProfile : Profile
    {
        public CreateRoleMapProfile()
        {
            CreateMap<CreateRoleCommand, ApplicationRole>();
        }
    }

}
