using Authentication.Application.Dtos;
using Authentication.Application.Handlers;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        ISignInHandler signInHandler,
        ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, Result<AuthenticationDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ISignInHandler _signInHandler = signInHandler;
        private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;

        public async Task<Result<AuthenticationDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.UserNotFound());
            }

            return await _signInHandler.HandleAsync(
                user,
                request.UserAgent,
                request.UserIp,
                request.RefreshToken,
                request.DeviceId,
                cancellationToken);
        }
    }
}
