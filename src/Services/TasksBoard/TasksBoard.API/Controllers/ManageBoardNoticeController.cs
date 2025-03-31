using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNoticeStatus;
using TasksBoard.Application.Models.Requests.ManageBoardNotices;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [HasBoardAccess]
    [HasBoardPermission("manage_notice")]
    [Route("api/managenotices")]
    public class ManageBoardNoticeController(
        IMediator mediator,
        ILogger<ManageBoardNoticeController> logger) : ControllerBase
    {
        private readonly ILogger<ManageBoardNoticeController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpPost("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardNoticeAsync([FromRoute] Guid boardId, CreateBoardNoticeRequest request)
        {
            var command = new CreateBoardNoticeCommand
            {
                BoardId = boardId,
                AuthorId = request.AuthorId,
                Definition = request.Definition,
                NoticeStatusId = request.NoticeStatusId,
                BackgroundColor = request.BackgroundColor,
                Rotation = request.Rotation
            };

            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            var locationUrl = Url.Action(
                action: nameof(BoardController.GetBoardByIdAsync),
                controller: nameof(BoardController),
                values: new { boardId },
                protocol: Request.Scheme
                );

            return Created(locationUrl, response);
        }

        [HttpPut("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardNoticeAsync([FromRoute] Guid boardId, UpdateBoardNoticeRequest request)
        {
            var command = new UpdateBoardNoticeCommand
            {
                BoardId = boardId,
                NoticeId = request.NoticeId,
                Definition = request.Definition,
                BackgroundColor = request.BackgroundColor,
                Rotation = request.Rotation
            };

            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }

        [HttpPut("status/board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardNoticeStatusAsync([FromRoute] Guid boardId, UpdateBoardNoticeStatusRequest request)
        {
            var command = new UpdateBoardNoticeStatusCommand
            {
                BoardId = boardId,
                NoticeId = request.NoticeId,
                Complete = request.Complete
            };

            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }

        [HttpDelete("board/{boardId:guid}/notice/{noticeId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBoardNoticeAsync([FromRoute] Guid boardId, [FromRoute] Guid noticeId)
        {
            await _mediator.Send(new DeleteBoardNoticeCommand
            {
                BoardId = boardId,
                NoticeId = noticeId
            });

            var response = new Response();

            return Ok(response);
        }
    }
}
