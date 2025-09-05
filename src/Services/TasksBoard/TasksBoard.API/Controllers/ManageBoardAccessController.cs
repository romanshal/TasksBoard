using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.ManageBoardAccessRequests;
using TasksBoard.Application.Features.ManageBoardAccesses.Commands.ResolveAccessRequest;
using TasksBoard.Application.Features.ManageBoardAccesses.Queries.GetBoardAccessRequestsByBoardId;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [HasBoardAccess]
    [Route("api/manageaccessrequests")]
    public class ManageBoardAccessController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("resolve/board/{boardId:guid}")]
        [HasBoardPermission("manage_member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResolveAccessRequestAsync([FromRoute] Guid boardId, [FromBody] ResolveAccessRequestRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(new ResolveAccessRequestCommand
            {
                BoardId = boardId,
                RequestId = request.RequestId,
                ResolveUserId = Guid.Parse(userId!),
                Decision = request.Decision
            }, cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpGet("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardAccessRequestsByBoarIdAsync([FromRoute] Guid boardId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBoardAccessRequestsByBoardIdQuery
            {
                BoardId = boardId
            }, cancellationToken);

            return this.HandleResponse(result);
        }
    }
}
