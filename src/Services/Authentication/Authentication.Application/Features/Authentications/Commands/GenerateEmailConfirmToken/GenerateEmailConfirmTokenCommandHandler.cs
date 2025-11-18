using Authentication.Application.Dtos;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Constants.Emails;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Factories;
using Authentication.Domain.Interfaces.Handlers;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Application.Features.Authentications.Commands.GenerateEmailConfirmToken
{
    internal sealed class GenerateEmailConfirmTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailMessageFactory emailMessageFactory,
        IEmailHandler emailHandler) : IRequestHandler<GenerateEmailConfirmTokenCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailMessageFactory _emailMessageFactory = emailMessageFactory;
        private readonly IEmailHandler _emailHandler = emailHandler;

        public async Task<Result> Handle(GenerateEmailConfirmTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.UserNotFound());
            }
            else if (user.EmailConfirmed)
            {
                Result.Success();
            }

            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var message = _emailMessageFactory.Create(EmailType.ConfirmEmail, user, confirmEmailToken);

            await _emailHandler.HandleAsync(message, cancellationToken);

            return Result.Success();
        }
    }
}