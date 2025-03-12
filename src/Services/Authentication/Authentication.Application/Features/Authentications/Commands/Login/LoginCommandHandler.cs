using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Authentication.Application.Providers;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public class LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenProvider tokenProvider,
        ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly TokenProvider _tokenProvider = tokenProvider;
        private readonly ILogger<LoginCommandHandler> _logger = logger;

        public async Task<AuthenticationDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
            {
                _logger.LogWarning($"User with name {request.Username} not found.");
                throw new UnauthorizedException($"User with name {request.Username} not found.");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, false);
            if (!signInResult.Succeeded)
            {
                _logger.LogWarning($"Signin faulted for user: {request.Username}.");
                throw new SigninFaultedException();
            }
            else if (signInResult.IsLockedOut)
            {
                _logger.LogWarning($"Signin is locked for user: {request.Username}.");
                throw new LockedException();
            }
            else if (signInResult.IsNotAllowed)
            {
                _logger.LogWarning($"Signin is not allowed for user {request.Username}.");
                throw new Exception($"Signin is not allowed for user {request.Username}."); //TODO: change this later
            }

            var token = _tokenProvider.Create(new CreateTokenModel { UserId = user.Id, UserEmail = user.Email! });

            var saveTokenResult = await _userManager.SetAuthenticationTokenAsync(user, JwtConstants.TokenType, "refresh_token", token.RefreshToken);
            if (!saveTokenResult.Succeeded)
            {
                _logger.LogCritical($"Can't save refresh token for user: {request.Username}. Errors: {string.Join("; ", saveTokenResult.Errors)}");
                throw new Exception($"Can't save refresh token for user: {request.Username}. Errors: {string.Join("; ", saveTokenResult.Errors)}");
            }

            _logger.LogInformation($"Success signin for user: {request.Username}.");

            return token;
        }
    }
}
