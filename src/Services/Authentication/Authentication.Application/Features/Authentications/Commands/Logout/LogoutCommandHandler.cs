using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.Logout
{
    public class LogoutCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenManager tokenService,
        ILogger<LogoutCommandHandler> logger) : IRequestHandler<LogoutCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ITokenManager _tokenService = tokenService;
        private readonly ILogger<LogoutCommandHandler> _logger = logger;

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result.Failure(AuthenticationErrors.UserNotFound());
            }

            await _userManager.UpdateSecurityStampAsync(user);

            await _tokenService.RevokeAsync(user, cancellationToken);

            await _signInManager.SignOutAsync();

            return Result.Success();
        }
    }
}
