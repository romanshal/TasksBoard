using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Authentications
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
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
        }
    }
}
