using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.BoardInviteRequests;
using TasksBoard.API.Models.Requests.ManageBoardInviteRequests;
using TasksBoard.Application.Features.ManageBoardInvites.Command.CancelInviteRequest;
using TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest;
using TasksBoard.Application.Features.ManageBoardInvites.Queries.GetBoardInviteRequestsByBoardId;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [HasBoardAccess]
    [Route("api/manageinviterequests")]
    public class ManageBoardInviteController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("board/{boardId:guid}")]
        [HasBoardPermission("manage_member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardInviteRequestAsync([FromRoute] Guid boardId, [FromBody] CreateInviteRequestRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CreateBoardInviteRequestCommand
            {
                BoardId = boardId,
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId
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
        public async Task<IActionResult> GetBoardInviteRequestsByBoarIdAsync([FromRoute] Guid boardId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBoardInviteRequestsByBoardIdQuery
            {
                BoardId = boardId
            }, cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpPut("board/{boardId:guid}")]
        [HasBoardPermission("manage_member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelBoardInviteRequestAsync([FromRoute] Guid boardId, [FromBody] CancelInviteRequestRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CancelInviteRequestCommand
            {
                RequestId = request.RequestId,
                BoardId = boardId
            }, cancellationToken);

            return this.HandleResponse(result);
        }
    }
}
