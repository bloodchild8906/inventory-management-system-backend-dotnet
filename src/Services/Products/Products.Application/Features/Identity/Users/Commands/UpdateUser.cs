using Application.Shared.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Products.Application.Domain.Identity;
using Products.Application.Shared.Constants;

namespace Products.Application.Features.Identity.Users.Commands
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UpdateUserHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task<string> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            var newRole = await _roleManager.FindByIdAsync(request.RoleId);
            
            if (user.Email == UserConstants.AdminEmail)
            {
                throw new IdentityException("The admin user cannot be modified.");
            }

            if (user == null)
            {
                throw new NotFoundException($"The User with the Id {request.UserId} was not found.");
            }

            if (newRole == null)
            {
                throw new NotFoundException($"The Role with the Id {request.RoleId} was not found.");
            }

            user.Fullname = request.Fullname;
            user.RoleId = request.RoleId;

            await _userManager.UpdateAsync(user);
            return user.Id;
        }
    }

    public class UpdateUserCommand : IRequest<string>
    {
        public string UserId { get; set; } = default!;
        public string RoleId { get; set; } = default!;
        public string Fullname { get; set; } = default!;
    }

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(r => r.UserId)
                .NotNull()
                .NotEmpty()
                .WithMessage("'UserId' is required.");

            RuleFor(r => r.RoleId)
                .NotNull()
                .NotEmpty()
                .WithMessage("'RoleId' is required.");


            RuleFor(r => r.Fullname)
                .NotNull()
                .NotEmpty()
                .WithMessage("'Fullname' is required.");

            RuleFor(r => r.Fullname)
                .MinimumLength(2)
                .WithMessage("The length of the Fullname is too short. The minimum length must be 2 characters.");

            RuleFor(r => r.Fullname)
                .MaximumLength(50)
                .WithMessage("The length of the Fullname is too long. The maximum length must be 50 characters.");
        }
    }
}
