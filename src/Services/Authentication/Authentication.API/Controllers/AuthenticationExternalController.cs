using Authentication.Application.Features.Authentications.Commands.ExternalLogin;
using Authentication.Application.Features.Authentications.Commands.ExternalLoginCallback;
using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/authenticationexternal")]
    public class AuthenticationExternalController(
        IMediator mediator,
        ILogger<AuthenticationExternalController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<AuthenticationExternalController> _logger = logger;

        [HttpGet("external-login")]
        public async Task<IActionResult> ExternalLoginAsync([FromQuery] string provider, [FromQuery] string? returnUrl = null)
        {
            var result = await _mediator.Send(new ExternalLoginCommand 
            { 
                Provider = provider, 
                RedirectUrl = returnUrl 
            });

            return Challenge(result, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery] string deviceId, [FromQuery] string? returnUrl = null, [FromQuery] string? remoteError = null)
        {
            if (!string.IsNullOrEmpty(remoteError))
            {
                _logger.LogError("External login error: {remoteError}", remoteError);
                return BadRequest("External login failed");
            }

            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _mediator.Send(new ExternalLoginCallbackCommand 
            {
                UserIp = userIp,
                UserAgent = userAgent,
                DeviceId = deviceId
            });

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    ErrorCodes.AlreadyExist => Conflict(result.Error.Description),
                    _ => BadRequest(result.Error.Description),
                };
            }

            return Ok(result.Value);
        }
    }
}
