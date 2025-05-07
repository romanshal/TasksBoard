using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Application.Features.BoardAccesses.Commands.ResolveAccessRequest;
using TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestsByBoardId;
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

        [HttpPost("resolve/board/{boardId:guid}")]
        [HasBoardAccess]
        [HasBoardPermission("manage_member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResolveAccessRequestAsync([FromRoute] Guid boardId, [FromBody] ResolveAccessRequestRequest request)
        {
            var result = await _mediator.Send(new ResolveAccessRequestCommand
            {
                BoardId = boardId,
                RequestId = request.RequestId,
                Decision = request.Decision
            });

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }

        [HttpGet("board/{boardId:guid}")]
        [HasBoardAccess]
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
