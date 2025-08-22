using Authentication.Application.Features.Authentications.Commands.RefreshToken;
using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Authentications
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(AuthenticationMessages.UserIdRequired);

            RuleFor(p => p.RefreshToken)
                .NotNull()
                .NotEmpty()
                .WithMessage(AuthenticationMessages.RefreshTokenRequired);
        }
    }
}
