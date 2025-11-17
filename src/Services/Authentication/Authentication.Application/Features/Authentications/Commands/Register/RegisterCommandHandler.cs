using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.GenerateEmailConfirmToken;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Constants.Emails;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Handlers;
using Common.Blocks.Models.DomainResults;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.Register
{
    public class RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IMediator mediator,
        ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<RegisterCommandHandler> _logger = logger;

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is not null)
            {
                return Result.Failure(AuthenticationErrors.AlreadyExist(request.Username));
            }

            user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Username
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogCritical("Can't create new user with username: {username}. Errors: {errors}.", request.Username, string.Join("; ", createResult.Errors));
                return Result.Failure(AuthenticationErrors.SignupFaulted);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, "user");

            if (!addRoleResult.Succeeded)
            {
                _logger.LogCritical("Can't add role to user: {username}. Errors: {errors}.", request.Username, string.Join("; ", addRoleResult.Errors));
                return Result.Failure(AuthenticationErrors.SignupFaulted);
            }

            var result = await _mediator.Send(new GenerateEmailConfirmTokenCommand
            {
                UserId = user.Id,
            }, cancellationToken);

            if (result.IsFailure)
            {
                return Result.Failure(AuthenticationErrors.SignupFaulted);
            }

            _logger.LogDebug("Success register for user: {username}.", request.Username);

            return Result.Success();
        }
    }
}
