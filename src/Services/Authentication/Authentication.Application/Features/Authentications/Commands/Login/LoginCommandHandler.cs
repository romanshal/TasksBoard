using Authentication.Application.Dtos;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public class LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;
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

            var token = await _tokenService.GenerateTokenAsync(user);

            _logger.LogInformation($"Success signin for user: {request.Username}.");

            return token;
        }
    }
}
