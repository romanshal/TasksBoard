using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Models.Requests.BoardInviteRequests;
using TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest;
using TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/boardinviterequests")]
    public class BoardInviteController(
        ILogger<BoardInviteController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardInviteController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet("account/{accountId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardInviteRequestByToAccountIdAsync([FromRoute] Guid accountId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetBoardInviteRequestByToAccountIdQuery
            {
                AccountId = accountId
            }, cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpPost("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResolveBoardInviteRequestAsync([FromRoute] Guid boardId, ResolveInviteRequestRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new ResolveInviteRequestCommand
            {
                BoardId = boardId,
                RequestId = request.RequestId,
                Decision = request.Decision
            }, cancellationToken);

            return this.HandleResponse(result);
        }
    }
}
