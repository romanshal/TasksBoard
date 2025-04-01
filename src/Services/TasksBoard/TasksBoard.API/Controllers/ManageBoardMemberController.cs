using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember;
using TasksBoard.Application.Features.ManageBoardMembers.Queries;
using TasksBoard.Application.Models.Requests.ManageBoardMembers;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [HasBoardAccess]
    [HasBoardPermission("manage_member")]
    [Route("api/managemembers")]
    public class ManageBoardMemberController(
        IMediator mediator,
        ILogger<ManageBoardMemberController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ManageBoardMemberController> _logger = logger;

        [HttpGet("board/{boardId:guid}/member/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardMemberPermissionsAsync([FromRoute] Guid boardId, [FromRoute] Guid memberId)
        {
            var result = await _mediator.Send(new GetBoardMemberPermissionsQuery
            {
                BoardId = boardId,
                MemberId = memberId
            });

            var response = new ResultResponse<IEnumerable<BoardMemberPermissionDto>>(result);

            return Ok(response);
        }

        [HttpPost("board/{boardId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBoardMemberAsync([FromRoute] Guid boardId, AddBoardMemberRequest request)
        {
            var command = new AddBoardMemberCommand
            {
                BoardId = boardId,
                UserId = request.UserId,
                Permissions = request.Permissions
            };

            var result = await _mediator.Send(command);

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }

        [HttpPost("permissions/board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBoardPermissionsToBoardMemberAsync([FromRoute] Guid boardId, AddBoardMemberPermissionsRequest request)
        {
            var command = new AddBoardMemberPermissionsCommand
            {
                BoardId = boardId,
                MemberId = request.MemberId,
                Permissions = request.Permissions
            };

            await _mediator.Send(command);

            var response = new Response();

            return Ok(response);
        }

        [HttpDelete("board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBoardMemberAsync([FromRoute] Guid boardId, DeleteBoardMemberRequest request)
        {
            var command = new DeleteBoardMemberCommand
            {
                BoardId = boardId,
                MemberId = request.MemberId
            };

            await _mediator.Send(command);

            var response = new Response();

            return Ok(response);
        }
    }
}
