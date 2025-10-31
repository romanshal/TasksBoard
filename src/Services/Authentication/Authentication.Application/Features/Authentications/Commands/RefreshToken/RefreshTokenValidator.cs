using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Authentications.Commands.RefreshToken
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(BaseMessages.UserIdRequired);

            RuleFor(p => p.RefreshToken)
                .NotNull()
                .NotEmpty()
                .WithMessage(AuthenticationMessages.RefreshTokenRequired);
        }
    }
}
