using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByBoardId;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserIdAndBoardId;
using TasksBoard.Application.Models;

namespace TasksBoard.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/boardnotices")]
    public class BoardNoticeController(
        ILogger<BoardController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet("{pageIndex:int}/{pageSize:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardNoticesAsync(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedListQuery<BoardNoticeDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return this.HandleResponse(result);
        }

        [HttpGet("board/{boardId:Guid}/notice/{noticeId}")]
        [HasBoardAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardNoticeByBoardIdAndIdAsync([FromRoute] Guid boardId, [FromRoute] Guid noticeId)
        {
            var result = await _mediator.Send(new GetBoardNoticeByIdQuery { Id = noticeId });

            return this.HandleResponse(result);
        }

        [HttpGet("board/{boardId:Guid}")]
        [HasBoardAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardNoticesByBoardIdAsync([FromRoute] Guid boardId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardNoticesByBoardIdQuery
            {
                BoardId = boardId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return this.HandleResponse(result);
        }

        [HttpGet("user/{userId:Guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardNoticesByUserIdAsync([FromRoute] Guid userId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardNoticesByUserIdQuery
            {
                UserId = userId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return this.HandleResponse(result);
        }

        [HttpGet("user/{userId:Guid}/board/{boardId:Guid}")]
        [HasBoardAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardNoticesByUserIdAndBoardIdAsync([FromRoute] Guid userId, [FromRoute] Guid boardId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardNoticesByUserIdAndBoardIdQuery
            {
                UserId = userId,
                BoardId = boardId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return this.HandleResponse(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardNoticeByIdAsync([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetBoardNoticeByIdQuery { Id = id });

            return this.HandleResponse(result);
        }
    }
}
