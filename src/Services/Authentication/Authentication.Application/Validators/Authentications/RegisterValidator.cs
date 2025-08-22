using Authentication.Application.Features.Authentications.Commands.Register;
using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Authentications
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(p => p.Username)
                .NotNull()
                .NotEmpty()
                .WithMessage(AuthenticationMessages.UsernameRequired);

            RuleFor(p => p.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage(AuthenticationMessages.PasswordRequired)
                .MinimumLength(4)
                .WithMessage(AuthenticationMessages.PasswordMinLength);

            RuleFor(p => p.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(AuthenticationMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(AuthenticationMessages.EmailInvalid);
        }
    }
}
