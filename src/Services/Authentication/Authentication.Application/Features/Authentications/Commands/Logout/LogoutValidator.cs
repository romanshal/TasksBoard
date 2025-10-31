using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Authentications.Commands.Logout
{
    public class LogoutValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(BaseMessages.UserIdRequired);
        }
    }
}
