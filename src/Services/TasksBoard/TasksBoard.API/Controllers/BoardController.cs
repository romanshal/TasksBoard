using Common.Blocks.Extensions;
using Common.Blocks.Models.ApiResponses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksBoard.API.Attributes;
using TasksBoard.API.Models.Requests.Boards;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoards;
using TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoardsByUserId;
using TasksBoard.Application.Features.Boards.Queries.GetPaginatedPublicBoards;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/boards")]
    public class BoardController(
        ILogger<BoardController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet("{pageIndex:int}/{pageSize:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardsAsync(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardsQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            return this.HandleResponse(result);
        }

        [HttpGet("{boardId:guid}")]
        [HasBoardAccess]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBoardByIdAsync([FromRoute] Guid boardId)
        {
            var result = await _mediator.Send(new GetBoardByIdQuery { Id = boardId });

            return this.HandleResponse(result);
        }

        //TODO: add permission check
        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedBoardsByUserIdAsync([FromRoute] Guid userId, string? query, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPaginatedBoardsByUserIdQuery
            {
                UserId = userId,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Query = query
            });

            return this.HandleResponse(result);
        }

        [HttpGet("public")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedPublicBoardsAsync(int pageIndex = 1, int pageSize = 10)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(new GetPaginatedPublicBoardsQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                AccountId = Guid.Parse(userId)
            });

            return this.HandleResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBoardAsync([FromForm] CreateBoardRequest request)
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

            var result = await _mediator.Send(new CreateBoardCommand
            {
                OwnerId = request.OwnerId,
                OwnerNickname = request.OwnerNickname,
                Name = request.Name,
                Description = request.Description,
                Tags = request.Tags,
                Public = request.Public,
                Image = imageData,
                ImageExtension = imageExtension
            });

            return this.HandleResponse(result, () => Created(Url.Action(nameof(GetBoardByIdAsync), new { id = result.Value }), ApiResponse.Success(result.Value)));

            //if (result.IsFailure)
            //{
            //    return BadRequest(ApiResponse.Error(result.Error.Description));
            //}

            //var response = ApiResponse.Success(result.Value);

            //return Created(Url.Action(nameof(GetBoardByIdAsync), new { id = result.Value }), response);
        }
    }
}
