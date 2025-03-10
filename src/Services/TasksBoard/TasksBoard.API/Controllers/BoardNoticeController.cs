using MediatR;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.Application.Features.BoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Application.Features.BoardNotices.Commands.DeleteBoardCommand;
using TasksBoard.Application.Features.BoardNotices.Commands.UpdateBoardNotice;
using TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById;
using TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNotices;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNotices;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Route("api/boardnotices")]
    public class BoardNoticeController(ILogger<BoardController> logger, IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllBoardNoticeAsync()
        {
            var result = await _mediator.Send(new GetBoardNoticesQuery());
            return Ok(result);
        }

        [HttpGet("{boardId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardNoticesByBoardIdAsync([FromRoute] Guid boardId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardNoticesQuery {BoardId = boardId, PageIndex = pageIndex, PageSize = pageSize });
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardNoticeByIdAsync([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetBoardNoticeByIdQuery { Id = id });
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
        public async Task<IActionResult> CreateBoardNoticeAsync(CreateBoardNoticeCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            return Created(Url.Action(nameof(GetBoardNoticeByIdAsync), new { id = result }), command);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardNoticeAsync(UpdateBoardNoticeCommand command)
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
        public async Task<IActionResult> DeleteBoardNoticeAsync([FromRoute] Guid id)
        {
            await _mediator.Send(new DeleteBoardNoticeCommand { Id = id });
            return Ok();
        }
    }
}
