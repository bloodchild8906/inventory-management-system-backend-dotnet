using Inventory.Application.Features.Identity.Roles.Commands;
using Inventory.Application.Features.Identity.Roles.Queries;
using Inventory.Application.Shared.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RolesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        [Authorize(Policy = Permissions.Roles.Create)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            var roleId = await _mediator.Send(command);
            var createdResource = new { Id = roleId };
            var routeValues = new { roleId };
            return CreatedAtAction(nameof(GetById), routeValues, createdResource);
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        [HttpPut("{roleId}")]
        public async Task<IActionResult> Update(string roleId, [FromBody] UpdateRoleCommand command)
        {
            if (command.RoleId != roleId)
            {
                return BadRequest();
            }
            var updatedRoleId = await _mediator.Send(command);
            return Ok();
        }


        [Authorize(Policy = Permissions.Roles.Edit)]
        [HttpPut("{roleId}/Enable")]
        public async Task<IActionResult> Enable(string roleId)
        {
            var command = new EnableRoleCommand(roleId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        [HttpPut("{roleId}/Disable")]
        public async Task<IActionResult> Disable(string roleId)
        {
            var command = new DisableRoleCommand(roleId);
            await _mediator.Send(command);
            return Ok();
        }


        [Authorize(Policy = Permissions.Roles.Delete)]
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(string roleId)
        {
            var query = new DeleteRoleCommand(roleId);
            await _mediator.Send(query);
            return Ok();
        }

        [Authorize(Policy = Permissions.Roles.View)]
        [HttpGet("{roleId}/Permissions")]
        public async Task<IActionResult> GetPermissions(string roleId)
        {
            var query = new GetRolePermissionsQuery(roleId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [Authorize(Policy = Permissions.Roles.View)]
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetById(string roleId)
        {
            var query = new GetRoleByIdQuery(roleId);
            var role = await _mediator.Send(query);
            return Ok(role);
        }
        [Authorize(Policy = Permissions.Roles.View)]
        [HttpGet]
        public async Task<IActionResult> GetAllPaginated([FromQuery] GetRolesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Policy = Permissions.Roles.View)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRolesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
