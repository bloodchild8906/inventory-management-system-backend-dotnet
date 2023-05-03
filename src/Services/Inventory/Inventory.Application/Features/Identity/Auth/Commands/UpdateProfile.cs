using Application.Shared.Exceptions;
using FluentValidation;
using Inventory.Application.Domain.Identity;
using Inventory.Application.Extensions;
using Inventory.Application.Interfaces.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Application.Features.Identity.Auth.Commands
{
    public record UpdateProfileCommand(string Fullname, string Email, string? CurrentPassword, string? NewPassword) : IRequest<string>;

    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(r => r.Email)
                 .NotNull()
                 .NotEmpty()
                 .WithMessage("'Email' is required.");

            RuleFor(r => r.Email)
                .EmailAddress()
                .WithMessage("'Email' invalid format.");

            RuleFor(r => r.Email)
                .MaximumLength(50)
                .WithMessage("The length of the Email is too long. The maximum length must be 50 characters.");

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


            RuleFor(r => r.CurrentPassword)
                .MinimumLength(6)
                .WithMessage("The length of the CurrentPassword is too short. The minimum length must be 6 characters.");

            RuleFor(r => r.CurrentPassword)
                .MaximumLength(30)
                .WithMessage("The length of the CurrentPassword is too long. The maximum length must be 30 characters.");


            RuleFor(r => r.NewPassword)
                .MinimumLength(6)
                .WithMessage("The length of the New Password is too short. The minimum length must be 6 characters.");

            RuleFor(r => r.NewPassword)
                .MaximumLength(30)
                .WithMessage("The length of the New Password is too long. The maximum length must be 30 characters.");


        }
    }

    public class UpdateProfile : IRequestHandler<UpdateProfileCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public UpdateProfile(UserManager<ApplicationUser> userManager, IAuthenticatedUserService authenticatedUserService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _authenticatedUserService = authenticatedUserService ?? throw new ArgumentNullException(nameof(authenticatedUserService));
        }

        private async Task UpdatePassword(ApplicationUser user, UpdateProfileCommand request)
        {
            if (!string.IsNullOrWhiteSpace(request.CurrentPassword) && !string.IsNullOrWhiteSpace(request.NewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    throw new IdentityException(result.Errors.ToDictionary());
                }
            }
        }

        private async Task UpdateUser(ApplicationUser user, UpdateProfileCommand request)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new IdentityException(result.Errors.ToDictionary());
            }
        }

        public async Task<string> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_authenticatedUserService.UserId);

            if (user.Email != request.Email)
            {
                var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
                if (userWithSameEmail != null)
                {
                    throw new IdentityException($"Email {request.Email} is already registered.");
                }

                user.Email = request.Email;
                user.UserName = request.Email;
            }

            user.Fullname = request.Fullname;
            await UpdatePassword(user, request);
            await UpdateUser(user, request);

            return user.Id;
        }
    }
}
