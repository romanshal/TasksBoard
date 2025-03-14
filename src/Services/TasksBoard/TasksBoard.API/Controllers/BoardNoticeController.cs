using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.BoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Application.Features.BoardNotices.Commands.DeleteBoardCommand;
using TasksBoard.Application.Features.BoardNotices.Commands.UpdateBoardNotice;
using TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById;
using TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNotices;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNotices;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByBoardId;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Route("api/boardnotices")]
    public class BoardNoticeController(
        ILogger<BoardController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllBoardNoticeAsync()
        {
            var result = await _mediator.Send(new GetBoardNoticesQuery());

            var response = new ResultResponse<IEnumerable<BoardNoticeDto>>(result);

            return Ok(response);
        }

        [HttpGet("{pageIndex:int}/{pageSize:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardNoticesAsync(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardNoticesQuery 
            { 
                PageIndex = pageIndex, 
                PageSize = pageSize 
            });

            var response = new ResultResponse<PaginatedList<BoardNoticeDto>>(result);

            return Ok(response);
        }

        [HttpGet("board/{boardId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardNoticesByBoardIdAsync([FromRoute] Guid boardId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardNoticesByBoardIdQuery 
            { 
                BoardId = boardId,
                PageIndex = pageIndex, 
                PageSize = pageSize 
            });

            var response = new ResultResponse<PaginatedList<BoardNoticeDto>>(result);

            return Ok(response);
        }

        [HttpGet("user/{userId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardNoticesByUserIdAsync([FromRoute] Guid userId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardNoticesByUserIdQuery
            {
                UserId = userId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            var response = new ResultResponse<PaginatedList<BoardNoticeDto>>(result);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardNoticeByIdAsync([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetBoardNoticeByIdQuery { Id = id });
            if (result is null)
            {
                return NotFound();
            }

            var response = new ResultResponse<BoardNoticeDto>(result);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardNoticeAsync(CreateBoardNoticeCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Created(Url.Action(nameof(GetBoardNoticeByIdAsync), new { id = result }), response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardNoticeAsync(UpdateBoardNoticeCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBoardNoticeAsync([FromRoute] Guid id)
        {
            await _mediator.Send(new DeleteBoardNoticeCommand { Id = id });

            var response = new Response();

            return Ok(response);
        }
    }
}
