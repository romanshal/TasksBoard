using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Constants.TwoFactor;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Application.Features.Authentications.Commands.GenerateTwoFactorCode
{
    internal class GenerateTwoFactorCodeCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITwoFactorCodeSender sender) : IRequestHandler<GenerateTwoFactorCodeCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITwoFactorCodeSender _sender = sender;

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

            string address;
            if (request.Provider == TokenOptions.DefaultEmailProvider)
            {
                address = user.Email!;
            }
            else if (request.Provider == TokenOptions.DefaultPhoneProvider)
            {
                var phone = await _userManager.GetPhoneNumberAsync(user);
                if (string.IsNullOrEmpty(phone))
                {
                    return Result.Failure(TwoFactorErrors.NoPhone);
                }

                address = phone;
            }
            else
            {
                return Result.Failure(TwoFactorErrors.InvalidProvider);
            }

            var result = await _sender.SendAsync(request.Provider, address, token);

            if (!result) 
                return Result.Failure(TwoFactorErrors.CantSend);
            else 
                return Result.Success();
        }
    }
}
