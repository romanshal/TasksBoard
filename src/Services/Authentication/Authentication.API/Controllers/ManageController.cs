using Authentication.API.Models.Requests.Manage;
using Authentication.Application.Features.Manage.Commands.ChangeUserPassword;
using Authentication.Application.Features.Manage.Commands.UpdateUserImage;
using Authentication.Application.Features.Manage.Commands.UpdateUserInfo;
using Authentication.Application.Features.Manage.Queries.GetUserImage;
using Authentication.Application.Features.Manage.Queries.GetUserInfo;
using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/manage")]
    public class ManageController(
        IMediator mediator,
        ILogger<ManageController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ManageController> _logger = logger;

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserInfoAsync([FromRoute] Guid userId)
        {
            var result = await _mediator.Send(new GetUserInfoQuery { UserId = userId });
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("image/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserImageAsync([FromRoute] Guid userId)
        {
            var result = await _mediator.Send(new GetUserImageQuery { UserId = userId });

            return Ok(result);
        }

        [HttpPost("info/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserInfoAsync([FromRoute] Guid userId, UpdateUserInfoRequest request)
        {
            var result = await _mediator.Send(new UpdateUserInfoCommand
            {
                UserId = userId,
                Username = request.Username,
                Email = request.Email,
                Firstname = request.Firstname,
                Surname = request.Surname
            });

            return Ok(result);
        }

        [HttpPost("password/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUserPasswordAsync([FromRoute] Guid userId, ChangeUserPasswordRequest request)
        {
            var result = await _mediator.Send(new ChangeUserPasswordCommand
            {
                UserId = userId,
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            });

            return Ok(result);
        }

        [HttpPost("image/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserImageAsync([FromRoute] Guid userId, [FromForm] UpdateUserImageRequest request)
        {
            byte[]? imageData = null;
            string? imageExtension = string.Empty;
            if (request.Image == null && request.Image.Length == 0)
            {
                return BadRequest();
            }

            using var ms = new MemoryStream();
            await request.Image.CopyToAsync(ms);
            imageData = ms.ToArray();

            imageExtension = Path.GetExtension(request.Image.FileName);

            var result = await _mediator.Send(new UpdateUserImageCommand
            {
                UserId = userId,
                Image = imageData,
                ImageExtension = imageExtension
            });

            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            var response = new ResultResponse<Guid>(result);

            return Ok(response);
        }
    }
}
