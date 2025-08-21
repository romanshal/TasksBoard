using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.Application.Features.BoardPermission.Queries.GetBoardPermissions;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/permissions")]
    public class BoardPermissionController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardPermissionsAsync()
        {
            var result = await _mediator.Send(new GetBoardPermissionsQuery());

            return this.HandleResponse(result);
        }
    }
}
