using MediatR;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.Application.Features.BoardNoticeStatuses.Queries.GetBoardNoticeStatuses;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Route("api/noticestatuses")]
    public class BoardNoticeStatusController(ILogger<BoardNoticeStatusController> logger, IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardNoticeStatusController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllNoticeStatusesAsync()
        {
            var result = await _mediator.Send(new GetBoardNoticeStatusesQuery());
            if (!result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }
    }
}
