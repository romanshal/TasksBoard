using Chat.API.Hubs;
using Chat.API.Models.Requests;
using Chat.Application.Features.BoardMessages.Commands.CreateBoardMessage;
using Chat.Application.Features.BoardMessages.Commands.DeleteBoardMessage;
using Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage;
using Chat.Application.Features.BoardMessages.Queries.GetBoardMessagesByBoardId;
using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/boardmessages")]
    public class BoardMessageController(
        ILogger<BoardMessageController> logger,
        IMediator mediator,
        IHubContext<BoardChatHub> hubContext) : ControllerBase
    {
        private readonly ILogger<BoardMessageController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet("{boardId:guid}")]
        //[HasBoardAccess] TODO: add grpc connect
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardMessagesByBoardIdAsync([FromRoute] Guid boardId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetBoardMessagesByBoardIdQuery
            {
                BoardId = boardId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return this.HandleResponse(result);
        }

        [HttpPost("{boardId:guid}")]
        //[HasBoardAccess] TODO: add grpc connect
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardMessageAsync([FromRoute] Guid boardId, [FromBody] CreateBoardMessageRequest request)
        {
            var result = await _mediator.Send(new CreateBoardMessageCommand
            {
                BoardId = boardId,
                MemberId = request.MemberId,
                AccountId = request.AccountId,
                Message = request.Message
            });

            await hubContext.Clients.Group(boardId.ToString()).SendAsync("ReceiveMessage", result);

            return this.HandleResponse(result);
        }

        [HttpPut("{boardId:guid}")]
        //[HasBoardAccess] TODO: add grpc connect
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardMessageAsync([FromRoute] Guid boardId, [FromBody] UpdateBoardMessageRequest request)
        {
            var result = await _mediator.Send(new UpdateBoardMessageCommand
            {
                BoardId = boardId,
                BoardMessageId = request.BoardMessageId,
                Message = request.Message
            });

            await hubContext.Clients.Group(boardId.ToString()).SendAsync("EditMessage", result);

            return this.HandleResponse(result);
        }

        [HttpDelete("{boardId:guid}/{messageId:guid}")]
        //[HasBoardAccess] TODO: add grpc connect
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBoardMessageAsync([FromRoute] Guid boardId, [FromRoute] Guid messageId)
        {
            var result = await _mediator.Send(new DeleteBoardMessageCommand
            {
                BoardId = boardId,
                BoardMessageId = messageId,
            });

            await hubContext.Clients.Group(boardId.ToString()).SendAsync("DeleteMessage", messageId);

            return this.HandleResponse(result);
        }
    }
}
