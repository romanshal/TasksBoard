using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardPermission.Queries.GetBoardPermissions;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/permissions")]
    public class BoardPermissionController(
        ILogger<BoardController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardController> _logger = logger;
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

            var response = new ResultResponse<IEnumerable<BoardPermissionDto>>(result);

            return Ok(response);
        }
    }
}
