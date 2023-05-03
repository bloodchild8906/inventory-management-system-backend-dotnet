using Inventory.Application.Features.Identity.Users.Commands;
using Inventory.Application.Features.Identity.Users.Queries;
using Inventory.Application.Shared.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers.Identity
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Authorize(Policy = Permissions.Users.Create)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var userId = await _mediator.Send(command);
            var createdResource = new { Id = userId };
            var routeValues = new { userId };
            return CreatedAtAction(nameof(GetById), routeValues, createdResource);
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(string userId, [FromBody] UpdateUserCommand command)
        {
            if (command.UserId != userId)
            {
                return BadRequest();
            }
            var result = await _mediator.Send(command);
            return Ok();
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPut("{userId}/Enable")]
        public async Task<IActionResult> Enable(string userId)
        {
            var command = new EnableUserCommand { UserId = userId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPut("{userId}/Disable")]
        public async Task<IActionResult> Disable(string userId)
        {
            var command = new DisableUserCommand { UserId = userId };
            await _mediator.Send(command);
            return Ok();
        }

        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(string userId)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var user = await _mediator.Send(query);
            return Ok(user);
        }


        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Policy = Permissions.Users.Delete)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            await _mediator.Send(new DeleteUserCommand(userId));
            return Ok();
        }
    }
}
