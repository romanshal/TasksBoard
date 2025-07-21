using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.ManageBoardMembers;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember;
using TasksBoard.Application.Features.ManageBoardMembers.Queries;

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

        //[HttpPost("board/{boardId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> AddBoardMemberAsync([FromRoute] Guid boardId, AddBoardMemberRequest request)
        //{
        //    var command = new AddBoardMemberCommand
        //    {
        //        BoardId = boardId,
        //        AccountId = request.AccountId,
        //        Nickname = request.Nickname
        //    };

        //    var result = await _mediator.Send(command);

        //    var response = new ResultResponse<Guid>(result);

        //    return Ok(response);
        //}

        [HttpPost("permissions/board/{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBoardPermissionsToBoardMemberAsync([FromRoute] Guid boardId, AddBoardMemberPermissionsRequest request)
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value!;

            var command = new AddBoardMemberPermissionsCommand
            {
                BoardId = boardId,
                MemberId = request.MemberId,
                AccountId = Guid.Parse(userId),
                Permissions = request.Permissions
            };

            await _mediator.Send(command);

            var response = new Response();

            return Ok(response);
        }

        [HttpDelete("board/{boardId:guid}/member/{memberId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBoardMemberAsync([FromRoute] Guid boardId, [FromRoute] Guid memberId)
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var command = new DeleteBoardMemberCommand
            {
                BoardId = boardId,
                MemberId = memberId,
                RemoveByUserId = Guid.Parse(userId!)
            };

            await _mediator.Send(command);

            var response = new Response();

            return Ok(response);
        }
    }
}
