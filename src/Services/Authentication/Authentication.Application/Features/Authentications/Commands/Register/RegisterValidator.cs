using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Authentications.Commands.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(p => p.Username)
                .NotNull()
                .NotEmpty()
                .WithMessage(BaseMessages.UsernameRequired);

            RuleFor(p => p.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage(AuthenticationMessages.PasswordRequired)
                .MinimumLength(4)
                .WithMessage(AuthenticationMessages.PasswordMinLength);

            RuleFor(p => p.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(BaseMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(BaseMessages.EmailInvalid);
        }
    }
}
