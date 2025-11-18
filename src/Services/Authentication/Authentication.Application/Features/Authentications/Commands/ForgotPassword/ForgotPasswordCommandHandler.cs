using Authentication.Domain.Constants.Emails;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Factories;
using Authentication.Domain.Interfaces.Handlers;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Authentication.Application.Features.Authentications.Commands.ForgotPassword
{
    internal class ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailHandler emailHandler,
        IEmailMessageFactory emailMessageFactory,
        ILogger<ForgotPasswordCommandHandler> logger) : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailHandler _emailHandler = emailHandler;
        private readonly IEmailMessageFactory _emailMessageFactory = emailMessageFactory;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger = logger;

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
            {
                // Не выдаём информацию о существовании аккаунта
                _logger.LogInformation("Password reset requested for non-existing or unconfirmed email: {Email}", request.Email);
                return Result.Success();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var message = _emailMessageFactory.Create(EmailType.ResetPassword, user, encoded);

            await _emailHandler.HandleAsync(message, cancellationToken);

            _logger.LogInformation("Password reset email sent to {Email}", request.Email);

            return Result.Success();
        }
    }
}
