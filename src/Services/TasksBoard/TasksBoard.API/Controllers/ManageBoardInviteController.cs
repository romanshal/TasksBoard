﻿using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.ManageBoardInviteRequests;
using TasksBoard.Application.Features.ManageBoardInvites.Command.CreateBoardInviteRequest;
using TasksBoard.Application.Features.ManageBoardInvites.Queries.GetBoardInviteRequestsByBoardId;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [HasBoardAccess]
    [HasBoardPermission("manage_member")]
    [Route("api/manageinviterequests")]
    public class ManageBoardInviteController(
        ILogger<ManageBoardInviteController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<ManageBoardInviteController> _logger = logger;
        private readonly IMediator _mediator = mediator;


        [HttpPost("board/{boardId:guid}")]
        [HasBoardAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardInviteRequestAsync([FromRoute] Guid boardId, [FromBody] CreateInviteRequestRequest request)
        {
            var result = await _mediator.Send(new CreateBoardInviteRequestCommand
            {
                BoardId = boardId,
                FromAccountId = request.FromAccountId,
                FromAccountName = request.FromAccountName,
                ToAccountId = request.ToAccountId,
                ToAccountName = request.ToAccountName,
                ToAccountEmail = request.ToAccountEmail
            });

            return this.HandleResponse(result);
        }

        [HttpGet("board/{boardId:guid}")]
        [HasBoardAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardInviteRequestsByBoarIdAsync([FromRoute] Guid boardId)
        {
            var result = await _mediator.Send(new GetBoardInviteRequestsByBoardIdQuery
            {
                BoardId = boardId
            });

            return this.HandleResponse(result);
        }
    }
}
