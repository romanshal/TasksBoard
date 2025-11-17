using Authentication.API.Extensions;
using Authentication.API.Models.Requests.Authentication;
using Authentication.Application.Features.Authentications.Commands.ConfirmEmail;
using Authentication.Application.Features.Authentications.Commands.ForgotPassword;
using Authentication.Application.Features.Authentications.Commands.GenerateEmailConfirmToken;
using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Application.Features.Authentications.Commands.Logout;
using Authentication.Application.Features.Authentications.Commands.RefreshToken;
using Authentication.Application.Features.Authentications.Commands.Register;
using AutoMapper;
using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController(
        IMediator mediator,
        IMapper mapper) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _mediator.Send(new LoginCommand
            {
                UsernameOrEmail = request.UsernameOrEmail,
                Password = request.Password,
                UserIp = userIp,
                UserAgent = userAgent,
                RememberMe = request.RememberMe
            }, cancellationToken);

            return this.MapResponse(result, _mapper);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
            }, cancellationToken);

            if (result.IsFailure)
            {
                this.MapErrors(result.Error);
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("generate-confirm-email")]
        public async Task<IActionResult> GenerateConfirmEmailAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GenerateEmailConfirmTokenCommand
            {
                UserId = userId
            }, cancellationToken);

            if (result.IsFailure)
            {
                this.MapErrors(result.Error);
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] Guid userId, [FromQuery] string token, CancellationToken cancellationToken = default)
        {
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _mediator.Send(new ConfirmEmailCommand
            {
                UserAgent = userAgent,
                UserIp = userIp,
                UserId = userId,
                Token = token
            }, cancellationToken);

            return this.MapResponse(result, _mapper);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
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
            }, cancellationToken);

            return this.MapResponse(result, _mapper);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                this.MapErrors(result.Error);
            }

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new LogoutCommand
            {
                UserId = userId
            }, cancellationToken);

            if (result.IsFailure)
            {
                this.MapErrors(result.Error);
            }

            Response.Cookies.Delete("refresh_token");

            return NoContent();
        }
    }
}
