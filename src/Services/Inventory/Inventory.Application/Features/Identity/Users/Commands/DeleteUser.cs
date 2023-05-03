using Application.Shared.Exceptions;
using Inventory.Application.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Application.Features.Identity.Users.Commands
{
    public record DeleteUserCommand(string UserId) : IRequest<Unit>;

    public class DeleteUser : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUser(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException($"The User with the Id {request.UserId} was not found.");
            }
            await _userManager.DeleteAsync(user);
            return Unit.Value;
        }
    }
}
