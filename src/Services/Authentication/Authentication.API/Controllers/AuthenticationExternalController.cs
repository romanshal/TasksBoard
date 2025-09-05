using Authentication.API.Extensions;
using Authentication.Application.Features.Authentications.Commands.ExternalLogin;
using Authentication.Application.Features.Authentications.Commands.ExternalLoginCallback;
using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

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

        [HttpGet("login")]
        public async Task<IActionResult> ExternalLoginAsync(
            [FromServices] LinkGenerator linkGenerator, 
            [FromQuery] string provider,
            [FromQuery] string? redirectUrl = "/", 
            CancellationToken cancellationToken = default)
        {
            var retrunUrl = linkGenerator.GetPathByAction(HttpContext, "ExternalLoginCallback") + $"?returnUrl={redirectUrl}";

            var result = await _mediator.Send(new ExternalLoginCommand 
            { 
                Provider = provider, 
                RedirectUrl = retrunUrl
            }, cancellationToken);

            return Challenge(result, ["Google"]);
        }

        [HttpGet("login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(
            [FromQuery] string returnUrl, 
            [FromQuery] string? remoteError = null, 
            CancellationToken cancellationToken = default)
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
                UserAgent = userAgent
            }, cancellationToken);

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    ErrorCodes.AlreadyExist => Conflict(result.Error.Description),
                    _ => BadRequest(result.Error.Description),
                };
            }

            var returnParams = new List<KeyValuePair<string, string?>>
            {
                new("accessToken", result.Value.AccessToken),
                new("accessTokenExpiredAt", result.Value.AccessTokenExpiredAt.ToString()),
                new("userId", result.Value.UserId.ToString()),
                new("deviceId", result.Value.DeviceId)
            };

            var url = QueryHelpers.AddQueryString(returnUrl, returnParams);

            this.SetRefreshTokenCookies(result.Value.RefreshToken);

            return Redirect(url);
        }
    }
}
