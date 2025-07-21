using Authentication.Application.Features.Authentications.Commands.Logout;
using Authentication.Domain.Constants.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Authentications
{
    public class LogoutValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(AuthenticationMessages.UserIdRequired);
        }
    }
}
