using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.ManageBoardAccessRequests;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.ManageBoardAccesses.Commands.ResolveAccessRequest;
using TasksBoard.Application.Features.ManageBoardAccesses.Queries.GetBoardAccessRequestsByBoardId;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [HasBoardAccess]
    [Route("api/manageaccessrequests")]
    public class ManageBoardAccessController(
        ILogger<ManageBoardAccessController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<ManageBoardAccessController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpPost("resolve/board/{boardId:guid}")]
        [HasBoardPermission("manage_member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResolveAccessRequestAsync([FromRoute] Guid boardId, [FromBody] ResolveAccessRequestRequest request)
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(new ResolveAccessRequestCommand
            {
                BoardId = boardId,
                RequestId = request.RequestId,
                ResolveUserId = Guid.Parse(userId!),
                Decision = request.Decision
            });

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }

        [HttpGet("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardAccessRequestsByBoarIdAsync([FromRoute] Guid boardId)
        {
            var result = await _mediator.Send(new GetBoardAccessRequestsByBoardIdQuery
            {
                BoardId = boardId
            });

            var response = new ResultResponse<IEnumerable<BoardAccessRequestDto>>(result);

            return Ok(response);
        }
    }
}
