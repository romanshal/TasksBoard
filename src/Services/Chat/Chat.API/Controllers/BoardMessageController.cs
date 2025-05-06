using Chat.API.Hubs;
using Chat.Application.DTOs;
using Chat.Application.Features.BoardMessages.Commands.CreateBoardMessage;
using Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage;
using Chat.Application.Features.BoardMessages.Queries.GetBoardMessagesByBoardId;
using Chat.Application.Models.Requests;
using Common.Blocks.Models;
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
            var result = await _mediator.Send(new GetBoardMessagesByBoardIdQuery {
                BoardId = boardId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            var response = new ResultResponse<IEnumerable<BoardMessageDto>>(result);

            return Ok(response);
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
                MemberNickname = request.MemberNickname,
                Message = request.Message
            });

            await hubContext.Clients.Group(boardId.ToString()).SendAsync("ReceiveMessage", result);

            var response = new ResultResponse<Guid>(result.Id);

            return Ok(response);
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



            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }
    }
}
