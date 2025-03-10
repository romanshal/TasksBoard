using MediatR;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Application.Features.Boards.Commands.DeleteBoardCommand;
using TasksBoard.Application.Features.Boards.Commands.UpdateBoard;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Features.Boards.Queries.GetBoards;
using TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoards;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Route("api/boards")]
    public class BoardController(ILogger<BoardController> logger, IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllBoardsAsync()
        {
            var result = await _mediator.Send(new GetBoardsQuery());
            if (!result.Any())
                return NoContent();

            return Ok(result);
        }

        [HttpGet("{pageIndex:int}/{pageSize:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardsAsync(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardsQuery { PageIndex = pageIndex, PageSize = pageSize });
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetBoardByIdQuery { Id = id });
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardAsync(CreateBoardCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            return Created(Url.Action(nameof(GetByIdAsync), new { id = result }), command);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardAsync(UpdateBoardCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBoardAsync([FromRoute] Guid id)
        {
            await _mediator.Send(new DeleteBoardCommand { Id = id });
            return Ok();
        }
    }
}
