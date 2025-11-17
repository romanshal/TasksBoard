using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.Register;
using Authentication.Application.Handlers;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.ConfirmEmail
{
    internal class ConfirmEmailCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        SignInHandler signInHandler,
        ILogger<RegisterCommandHandler> logger) : IRequestHandler<ConfirmEmailCommand, Result<AuthenticationDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        private readonly SignInHandler _signInHandler = signInHandler;
        private readonly ILogger<RegisterCommandHandler> _logger = logger;

        public async Task<Result<AuthenticationDto>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.UserNotFound());
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.EmailNotConfirmed);
            }

            await _signInManager.SignInAsync(user, false, "Password");

            return await _signInHandler.HandleAsync(user, request.UserAgent, request.UserIp, cancellationToken);
        }
    }
}
