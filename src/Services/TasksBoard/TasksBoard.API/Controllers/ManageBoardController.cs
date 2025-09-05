using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.ManageBoards;
using TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard;
using TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/manageboards")]
    [HasBoardAccess]
    [HasBoardPermission("manage_board")]
    public class ManageBoardController(
        IMediator mediator,
        ILogger<ManageBoardController> logger) : ControllerBase
    {
        private readonly ILogger<ManageBoardController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpPut("{boardId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBoardAsync([FromRoute] Guid boardId, [FromForm] UpdateBoardRequest request, CancellationToken cancellationToken = default)
        {
            byte[]? imageData = null;
            string? imageExtension = string.Empty;
            if (request.Image != null && request.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await request.Image.CopyToAsync(ms);
                imageData = ms.ToArray();

                imageExtension = Path.GetExtension(request.Image.FileName);
            }

            var command = new UpdateBoardCommand
            {
                BoardId = boardId,
                Name = request.Name,
                Description = request.Description,
                Tags = request.Tags,
                Public = request.Public,
                Image = imageData,
                ImageExtension = imageExtension
            };

            var result = await _mediator.Send(command, cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpDelete("{boardId:guid}")]
        [BoardOwner]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBoardAsync([FromRoute] Guid boardId, CancellationToken cancellationToken = default)
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value!;
            var result = await _mediator.Send(new DeleteBoardCommand
            {
                Id = boardId,
                AccountId = Guid.Parse(userId)
            }, cancellationToken);

            return this.HandleResponse(result);
        }
    }
}
