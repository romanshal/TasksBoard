using Authentication.Application.Features.Authentications.Commands.ExternalLogin;
using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Application.Features.Authentications.Commands.Logout;
using Authentication.Application.Features.Authentications.Commands.RefreshToken;
using Authentication.Application.Features.Authentications.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authentication.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/authentication")]
    public class AuthenticationController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route("external")]
        public async Task<IActionResult> ExternalLoginAsync(ExternalLoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> RefreshAsync(RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            if (!Guid.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new LogoutCommand
            {
                UserId = userId
            });

            return NoContent();
        }
    }
}
