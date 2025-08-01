﻿using Authentication.Application.Dtos;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;

        public async Task<AuthenticationDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User with name {userId} not found.", request.UserId);
                throw new UnauthorizedException($"User was not found.");
            }

            var token = await _tokenService.RefreshTokenAsync(user);
            if (token is null || string.IsNullOrEmpty(token?.AccessToken) || string.IsNullOrEmpty(token?.RefreshToken))
            {
                _logger.LogCritical("Can't create access or refresh tokens for user {id}.", user.Id);
                throw new UnauthorizedException("Can't create access or refresh tokens.");
            }

            return new AuthenticationDto
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                UserId = user.Id
            };
        }
    }
}
