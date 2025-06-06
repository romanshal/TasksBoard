using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest;
using TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId;
using TasksBoard.Application.Models.Requests.BoardInviteRequests;

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
        public async Task<IActionResult> GetBoardInviteRequestByToAccountIdAsync([FromRoute] Guid accountId)
        {
            var result = await _mediator.Send(new GetBoardInviteRequestByToAccountIdQuery
            {
                AccountId = accountId
            });

            var response = new ResultResponse<IEnumerable<BoardInviteRequestDto>>(result);

            return Ok(response);
        }

        [HttpPost("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResolveBoardInviteRequestAsync([FromRoute] Guid boardId, ResolveInviteRequestRequest request)
        {
            var result = await _mediator.Send(new ResolveInviteRequestCommand
            {
                BoardId = boardId,
                RequestId = request.RequestId,
                Decision = request.Decision
            });

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }
    }
}
