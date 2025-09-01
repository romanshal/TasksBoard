using Authentication.API.Extensions;
using Authentication.API.Models.Requests.Authentication;
using Authentication.API.Models.Responses;
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
    [Route("api/authentication")]
    public class AuthenticationController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _mediator.Send(new LoginCommand
            {
                Username = request.Username,
                Password = request.Password,
                UserIp = userIp,
                UserAgent = userAgent
            });

            this.SetRefreshTokenCookies(result.RefreshToken);

            return Ok(new AuthenticationResponse
            {
                UserId = result.UserId,
                AccessToken = result.AccessToken,
                ExpiredAt = result.AccessTokenExpiredAt,
                DeviceId = result.DeviceId
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("external")]
        public async Task<IActionResult> ExternalLoginAsync(ExternalLoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _mediator.Send(new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                UserIp = userIp,
                UserAgent = userAgent
            });

            this.SetRefreshTokenCookies(result.RefreshToken);

            return Ok(new AuthenticationResponse
            {
                UserId = result.UserId,
                AccessToken = result.AccessToken,
                ExpiredAt = result.AccessTokenExpiredAt,
                DeviceId = result.DeviceId
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request)
        {
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent.ToString();

            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken)) 
                return Unauthorized();

            var result = await _mediator.Send(new RefreshTokenCommand 
            {
                UserId = request.UserId,
                RefreshToken = refreshToken,
                UserIp = userIp,
                UserAgent = userAgent,
                DeviceId = request.DeviceId
            });

            this.SetRefreshTokenCookies(result.RefreshToken);

            return Ok(new AuthenticationResponse
            {
                UserId = result.UserId,
                AccessToken = result.AccessToken,
                ExpiredAt = result.AccessTokenExpiredAt,
                DeviceId = result.DeviceId
            });
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

            Response.Cookies.Delete("refresh_token");

            return NoContent();
        }
    }
}
