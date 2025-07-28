using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.ManageBoardMembers;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember;
using TasksBoard.Application.Features.ManageBoardMembers.Queries.GetBoardMemberPermissions;

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

            return this.HandleResponse(result);
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

            var result = await _mediator.Send(new AddBoardMemberPermissionsCommand
            {
                BoardId = boardId,
                MemberId = request.MemberId,
                AccountId = Guid.Parse(userId),
                Permissions = request.Permissions
            });

            return this.HandleResponse(result);
        }

        [HttpDelete("board/{boardId:guid}/member/{memberId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBoardMemberAsync([FromRoute] Guid boardId, [FromRoute] Guid memberId)
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(new DeleteBoardMemberCommand
            {
                BoardId = boardId,
                MemberId = memberId,
                RemoveByUserId = Guid.Parse(userId!)
            });

            return this.HandleResponse(result);
        }
    }
}
