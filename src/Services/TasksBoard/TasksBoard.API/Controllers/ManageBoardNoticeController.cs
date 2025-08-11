using Common.Blocks.Extensions;
using Common.Blocks.Models.ApiResponses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.ManageBoardNotices;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardNotice;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNoticeStatus;

namespace TasksBoard.API.Controllers
{
    [Authorize]
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardNoticeAsync([FromRoute] Guid boardId, CreateBoardNoticeRequest request)
        {
            var result = await _mediator.Send(new CreateBoardNoticeCommand
            {
                BoardId = boardId,
                AuthorId = request.AuthorId,
                AuthorName = request.AuthorName,
                Definition = request.Definition,
                BackgroundColor = request.BackgroundColor,
                Rotation = request.Rotation
            });

            var locationUrl = Url.Action(
                action: nameof(BoardController.GetBoardByIdAsync),
                controller: nameof(BoardController),
                values: new { boardId },
                protocol: Request.Scheme
                );

            return this.HandleResponse(result, () => Created(locationUrl, ApiResponse.Success(result.Value)));

            //if (result.IsFailure)
            //{
            //    return BadRequest(ApiResponse.Error(result.Error.Description));
            //}

            //var response = ApiResponse.Success(result);

            //var locationUrl = Url.Action(
            //    action: nameof(BoardController.GetBoardByIdAsync),
            //    controller: nameof(BoardController),
            //    values: new { boardId },
            //    protocol: Request.Scheme
            //    );

            //return Created(locationUrl, response);
        }

        [HttpPut("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardNoticeAsync([FromRoute] Guid boardId, UpdateBoardNoticeRequest request)
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value!;

            var result = await _mediator.Send(new UpdateBoardNoticeCommand
            {
                BoardId = boardId,
                NoticeId = request.NoticeId,
                AccountId = Guid.Parse(userId),
                Definition = request.Definition,
                BackgroundColor = request.BackgroundColor,
                Rotation = request.Rotation
            });

            return this.HandleResponse(result);
        }

        [HttpPut("status/board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardNoticeStatusAsync([FromRoute] Guid boardId, UpdateBoardNoticeStatusRequest request)
        {
            var result = await _mediator.Send(new UpdateBoardNoticeStatusCommand
            {
                BoardId = boardId,
                AccountId = request.AccountId,
                AccountName = request.AccountName,
                NoticeId = request.NoticeId,
                Complete = request.Complete
            });

            return this.HandleResponse(result);
        }

        [HttpDelete("board/{boardId:guid}/notice/{noticeId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBoardNoticeAsync([FromRoute] Guid boardId, [FromRoute] Guid noticeId)
        {
            var result = await _mediator.Send(new DeleteBoardNoticeCommand
            {
                BoardId = boardId,
                NoticeId = noticeId
            });

            return this.HandleResponse(result);
        }
    }
}
