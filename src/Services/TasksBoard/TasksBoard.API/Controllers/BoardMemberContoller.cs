using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMemberByBoardIdAndAccountId;
using TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMembersByBoardId;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [HasBoardAccess]
    [Route("api/boardmembers")]
    public class BoardMemberContoller(
        IMediator mediator,
        ILogger<BoardMemberContoller> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private ILogger<BoardMemberContoller> _logger = logger;

        [HttpGet("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardMembersByBoardIdAsync([FromRoute] Guid boardId)
        {
            var result = await _mediator.Send(new GetBoardMembersByBoardIdQuery
            {
                BoardId = boardId,
            });

            var response = new ResultResponse<IEnumerable<BoardMemberDto>>(result);

            return Ok(response);
        }

        [HttpGet("board/{boardId:guid}/account/{accountId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardMemberByBoardIdAndAccountIdAsync(Guid boardId, Guid accountId)
        {
            var result = await _mediator.Send(new GetBoardMemberByBoardIdAndAccountIdQuery
            {
                BoardId = boardId,
                AccountId = accountId
            });

            var response = new ResultResponse<BoardMemberDto>(result);

            return Ok(response);
        }

        //[HttpGet("board/{boardId:guid}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetPaginatedBoardMembersByBoardIdAsync([FromRoute] Guid boardId, int pageIndex = 1, int pageSize = 10)
        //{
        //    var result = await _mediator.Send(new GetPaginatedBoardMembersByBoardIdQuery
        //    {
        //        BoardId = boardId,
        //        PageIndex = pageIndex,
        //        PageSize = pageSize
        //    });

        //    var response = new ResultResponse<PaginatedList<BoardMemberDto>>(result);

        //    return Ok(response);
        //}
    }
}
