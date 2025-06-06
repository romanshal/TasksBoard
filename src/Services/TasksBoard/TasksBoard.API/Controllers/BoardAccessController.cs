using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId;
using TasksBoard.Application.Models.Requests.BoardAccessRequests;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/boardaccessrequests")]
    public class BoardAccessController(
        ILogger<BoardAccessController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardAccessController> _logger = logger;
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

            var response = new ResultResponse<IEnumerable<BoardAccessRequestDto>>(result);

            return Ok(response);
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
            var result = await _mediator.Send(new RequestBoardAccessQuery
            {
                BoardId = boardId,
                AccountId = request.AccountId,
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail
            });

            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
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

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }
    }
}
