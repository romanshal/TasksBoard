using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember;
using TasksBoard.Domain.Entities;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [HasBoardAccess]
    [HasBoardPermission("manage_members")]
    [Route("api/managemembers")]
    public class ManageBoardMemberController(
        IMediator mediator,
        ILogger<ManageBoardMemberController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ManageBoardMemberController> _logger = logger;

        [HttpPost("board/{boardId}")]
        public async Task<IActionResult> AddBoardMemberAsync([FromRoute] Guid boardId, AddBoardMemberCommand command)
        {
            command.BoardId = boardId;

            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }

        [HttpPost("board/{boardId:guid}/permissions")]
        public async Task<IActionResult> AddBoardPermissionsToBoardMemberAsync([FromRoute] Guid boardId, AddBoardPermissionsCommand command)
        {
            command.BoardId = boardId;

            await _mediator.Send(command);

            var response = new Response();

            return Ok(response);
        }

        [HttpDelete("board/{boardId:guid}")]
        public async Task<ActionResult> DeleteBoardMemberAsync([FromRoute] Guid boardId, DeleteBoardMemberCommand command)
        {
            command.BoardId = boardId;

            await _mediator.Send(command);

            var response = new Response();

            return Ok(response);
        }
    }
}
