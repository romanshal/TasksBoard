using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Common.Blocks.Exceptions;
using System.Security.Claims;
using Authentication.Application.Providers;
using Authentication.Application.Models;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Authentication.Application.Features.Authentications.Commands.Register
{
    public class RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenProvider tokenProvider,
        ILogger<LoginCommandHandler> logger) : IRequestHandler<RegisterCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly TokenProvider _tokenProvider = tokenProvider;
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

            var addClaimResult = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "user"));
            if (!addClaimResult.Succeeded)
            {
                _logger.LogCritical($"Can't add claim role to user: {request.Username}. Errors: {string.Join("; ", addClaimResult.Errors)}.");
                throw new Exception($"Can't add claim role to user: {request.Username}. Errors: {string.Join("; ", addClaimResult.Errors)}.");
            }

            await _signInManager.SignInAsync(user, false);

            var token = _tokenProvider.Create(new CreateTokenModel { UserId = user.Id, UserEmail = user.Email });

            var saveTokenResult = await _userManager.SetAuthenticationTokenAsync(user, JwtConstants.TokenType, "refresh_token", token.RefreshToken);
            if (!saveTokenResult.Succeeded)
            {
                _logger.LogCritical($"Can't save refresh token for user: {request.Username}. Errors: {string.Join("; ", saveTokenResult.Errors)}");
                throw new Exception($"Can't save refresh token for user: {request.Username}. Errors: {string.Join("; ", saveTokenResult.Errors)}");
            }

            _logger.LogInformation($"Success register for user: {request.Username}.");

            return token;
        }
    }
}
