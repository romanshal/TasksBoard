using Authentication.API.Attributes;
using Authentication.API.Models.Requests.Manage;
using Authentication.Application.Features.Manage.Commands.ChangeUserPassword;
using Authentication.Application.Features.Manage.Commands.UpdateUserImage;
using Authentication.Application.Features.Manage.Commands.UpdateUserInfo;
using Authentication.Application.Features.Manage.Queries.GetUserImage;
using Authentication.Application.Features.Manage.Queries.GetUserInfo;
using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/manage")]
    public class ManageController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserInfoAsync([FromRoute] Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUserInfoQuery { UserId = userId }, cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpGet("image/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserImageAsync([FromRoute] Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUserImageQuery { UserId = userId }, cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpPut("info/{userId:guid}")]
        [CurrentUserOnly]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserInfoAsync([FromRoute] Guid userId, UpdateUserInfoRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new UpdateUserInfoCommand
            {
                UserId = userId,
                Username = request.Username,
                Email = request.Email,
                Firstname = request.Firstname,
                Surname = request.Surname
            }, cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpPut("password/{userId:guid}")]
        [CurrentUserOnly]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUserPasswordAsync([FromRoute] Guid userId, ChangeUserPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new ChangeUserPasswordCommand
            {
                UserId = userId,
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            }, cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpPut("image/{userId:guid}")]
        [CurrentUserOnly]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserImageAsync([FromRoute] Guid userId, [FromForm] UpdateUserImageRequest request, CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream();
            await request.Image.CopyToAsync(ms, cancellationToken);
            byte[]? imageData = ms.ToArray();

            string? imageExtension = string.Empty;
            imageExtension = Path.GetExtension(request.Image.FileName);

            var result = await _mediator.Send(new UpdateUserImageCommand
            {
                UserId = userId,
                Image = imageData,
                ImageExtension = imageExtension
            }, cancellationToken);

            return this.HandleResponse(result);
        }
    }
}
