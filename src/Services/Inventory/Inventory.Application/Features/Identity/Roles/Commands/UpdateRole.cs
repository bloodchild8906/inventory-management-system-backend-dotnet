using Application.Shared.Exceptions;
using FluentValidation;
using Inventory.Application.Domain.Identity;
using Inventory.Application.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Features.Identity.Roles.Commands
{

    public record UpdateRoleModule(string Id, List<string> PermissionsIds);

    public record UpdateRoleCommand(string RoleId, string Name, string Description, List<UpdateRoleModule> Modules) : IRequest<string>;
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {

        private readonly ApplicationDbContext _context;

        public UpdateRoleCommandValidator(ApplicationDbContext context)
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

        private bool AllModulesAreUnique(List<UpdateRoleModule> modules)
        {
            var modulesIds = modules.Select(s => s.Id).ToList();
            return modules.Count == modulesIds.Distinct().Count();
        }

        private bool AllPermissionsAreUnique(List<UpdateRoleModule> modules)
        {
            var permissionsCount = modules.Sum(m => m.PermissionsIds.Count);
            var permissions = modules.SelectMany(m => m.PermissionsIds).ToList();
            return permissionsCount == permissions.Distinct().Count();
        }


        private async Task<bool> AllModulesExists(List<UpdateRoleModule> modules)
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

        private async Task<bool> AllPermissionsExists(List<UpdateRoleModule> modules)
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

        private bool AtLeastOneModuleSelected(List<UpdateRoleModule> modules)
        {
            return modules.Any(m => m.PermissionsIds != null && m.PermissionsIds.Count > 0);
        }

        private async Task<bool> AreRequiredPermissionsSelected(UpdateRoleModule module)
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


        private async Task<int> GetNumberOfValidModules(List<UpdateRoleModule> modules)
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

        private async Task<bool> AreSelectedModulesValid(List<UpdateRoleModule> modules)
        {
            var selectedModules = modules.Where(m => m.PermissionsIds != null && m.PermissionsIds.Count > 0).ToList();
            var numberOfValidModules = await GetNumberOfValidModules(modules);
            return numberOfValidModules == selectedModules.Count;
        }

    }

    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, string>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UpdateRoleHandler(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
            {
                throw new NotFoundException($"Role with the Id : '{request.RoleId}' was not found.");
            }

            if (!role.Active)
            {
                throw new IdentityException($"Role with the Id : '{request.RoleId}' it's disabled.");
            }

            if (!role.Name.Equals(request.Name))
            {
                var roleWithTheSameName = await _roleManager.FindByNameAsync(request.Name);

                if (roleWithTheSameName != null)
                {
                    throw new IdentityException($"The role with the Name {request.Name} already exists.");
                }
            }


            // Remove old permssions
            var oldPermissions = _context.RolePermissions.Where(p => p.RoleId == request.RoleId);
            _context.RemoveRange(oldPermissions);


            // Add new Permissions 
            var newPermissions = request.Modules.SelectMany(m => m.PermissionsIds).ToList();

            foreach (var permission in newPermissions)
            {
                var rolePermission = new RolePermission { PermissionId = permission, Role = role };
                _context.RolePermissions.Add(rolePermission);
            }

            await _context.SaveChangesAsync();


            role.Name = request.Name;
            role.Description = request.Description;
            await _roleManager.UpdateAsync(role);

            return role.Id;
        }
    }
}
