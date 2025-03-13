using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.Register
{
    public class RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger) : IRequestHandler<RegisterCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<LoginCommandHandler> _logger = logger;

        public async Task<AuthenticationDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is not null)
            {
                _logger.LogWarning($"User with name {request.Username} is already exist.");
                throw new AlreadyExistException($"User with name {request.Username} is already exist.");
            }

            user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Username
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogCritical($"Can't create new user with username: {request.Username}. Errors: {string.Join("; ", createResult.Errors)}.");
                throw new Exception($"Can't reate new user with username: {request.Username}. Errors: {string.Join("; ", createResult.Errors)}.");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, "user");

            if (!addRoleResult.Succeeded)
            {
                _logger.LogCritical($"Can't add role to user: {request.Username}. Errors: {string.Join("; ", addRoleResult.Errors)}.");
                throw new Exception($"Can't add role to user: {request.Username}. Errors: {string.Join("; ", addRoleResult.Errors)}.");
            }

            await _signInManager.SignInAsync(user, false);

            var token = await _tokenService.GenerateTokenAsync(user);

            _logger.LogInformation($"Success register for user: {request.Username}.");

            return token;
        }
    }
}
