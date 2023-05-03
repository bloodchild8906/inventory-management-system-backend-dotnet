using Inventory.Application.Features.Identity.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public IdentityController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> GetTokenAsync(GetTokenCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(token);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfileInfo([FromBody] UpdateProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok();
        }
    }
}
