using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoardsByUserId;
using TasksBoard.Application.Models;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Route("api/boards")]
    public class BoardController(
        ILogger<BoardController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet("{pageIndex:int}/{pageSize:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardsAsync(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedListQuery<BoardDto> { PageIndex = pageIndex, PageSize = pageSize });

            var response = new ResultResponse<PaginatedList<BoardDto>>(result);

            return Ok(response);
        }

        [HttpGet("{boardId:guid}")]
        [HasBoardAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardByIdAsync([FromRoute] Guid boardId)
        {
            var result = await _mediator.Send(new GetBoardByIdQuery { Id = boardId });
            if (result is null)
            {
                return NotFound();
            }

            var response = new ResultResponse<BoardDto>(result);

            return Ok(response);
        }

        //TODO: add permission check
        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardsByUserIdAsync([FromRoute] Guid userId, int pageIndex, int pageSize)
        {
            var result = await _mediator.Send(new GetPaginatedBoardsByUserIdQuery
            {
                UserId = userId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            if (result is null)
            {
                return NotFound();
            }

            var response = new ResultResponse<PaginatedList<BoardDto>>(result);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardAsync(CreateBoardCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Created(Url.Action(nameof(GetBoardByIdAsync), new { id = result }), response);
        }
    }
}
