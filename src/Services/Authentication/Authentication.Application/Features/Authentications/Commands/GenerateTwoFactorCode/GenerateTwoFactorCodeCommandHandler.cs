using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Constants.Emails;
using Authentication.Domain.Constants.TwoFactor;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Handlers;
using Common.Blocks.Models.DomainResults;
using EventBus.Messages.Abstraction.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Application.Features.Authentications.Commands.GenerateTwoFactorCode
{
    internal class GenerateTwoFactorCodeCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailHandler emailHandler) : IRequestHandler<GenerateTwoFactorCodeCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailHandler _emailHandler = emailHandler;

        public async Task<Result> Handle(GenerateTwoFactorCodeCommand request, CancellationToken cancellationToken)
        {
            if (request.Provider == TokenOptions.DefaultAuthenticatorProvider)
            {
                return Result.Failure(TwoFactorErrors.UseTOTP);
            }

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result.Failure(AuthenticationErrors.UserNotFound());
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, request.Provider);

            if (request.Provider == TokenOptions.DefaultEmailProvider)
            {
                var message = new EmailMessageEvent
                {
                    Recipient = user.Email!,
                    Subject = "2fa",
                    Body = EmailTexts.TwoFactor(token)
                };

                await _emailHandler.HandleAsync(message, cancellationToken);
            }
            else if (request.Provider == TokenOptions.DefaultPhoneProvider)
            {
                var phone = await _userManager.GetPhoneNumberAsync(user);
                if (string.IsNullOrEmpty(phone))
                {
                    return Result.Failure(TwoFactorErrors.NoPhone);
                }

                //TODO: add sms handler
            }
            else
            {
                return Result.Failure(TwoFactorErrors.InvalidProvider);
            }

            return Result.Success();
        }
    }
}
