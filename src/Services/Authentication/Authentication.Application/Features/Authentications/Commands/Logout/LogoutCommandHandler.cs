using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.Logout
{
    public class LogoutCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<LogoutCommandHandler> logger) : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<LogoutCommandHandler> _logger = logger;

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User with id '{userId}' not found.", request.UserId);
                throw new UnauthorizedException($"User with id '{request.UserId}' not found.");
            }

            var result = await _tokenService.DeleteRefreshToken(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Signout faulted for user: {userId}.", request.UserId);
                throw new SignoutFaultedException();
            }

            await _signInManager.SignOutAsync();

            return Unit.Value;
        }
    }
}
