using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TasksBoard.API.Attributes;
using TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard;
using TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [HasBoardAccess]
    [HasBoardPermission("manage_board")]
    public class ManageBoardController(
        IMediator mediator,
        Logger<ManageBoardController> logger) : ControllerBase
    {
        private readonly ILogger<ManageBoardController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpPut("{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardAsync([FromRoute] Guid boardId, UpdateBoardCommand command)
        {
            command.Id = boardId;

            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }

        [HttpDelete("{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBoardAsync([FromRoute] Guid boardId)
        {
            await _mediator.Send(new DeleteBoardCommand { Id = boardId });

            var response = new Response();

            return Ok(response);
        }
    }
}
