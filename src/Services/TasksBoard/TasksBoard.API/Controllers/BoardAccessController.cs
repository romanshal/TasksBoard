using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Models.Requests.BoardAccessRequests;
using TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/boardaccessrequests")]
    public class BoardAccessController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("account/{accountId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardAccessRequestByAccountIdAsync([FromRoute] Guid accountId)
        {
            var result = await _mediator.Send(new GetBoardAccessRequestByAccountIdQuery
            {
                AccountId = accountId
            });

            return this.HandleResponse(result);
        }

        [HttpPost("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestBoardAccessAsync([FromRoute] Guid boardId, [FromBody] RequestBoardAccessRequest request)
        {
            var result = await _mediator.Send(new RequestBoardAccessCommand
            {
                BoardId = boardId,
                AccountId = request.AccountId,
            });

            return this.HandleResponse(result);
        }

        [HttpPost("cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelAccessRequestAsync([FromBody] CancelBoardAccessCommand command)
        {
            var result = await _mediator.Send(command);

            return this.HandleResponse(result);
        }
    }
}
