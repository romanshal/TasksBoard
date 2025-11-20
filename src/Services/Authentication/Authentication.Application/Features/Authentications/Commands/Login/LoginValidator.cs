using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public partial class LoginValidator : AbstractValidator<LoginCommand>
    {
        [GeneratedRegex(@"^[A-Za-z0-9_-]{3,20}$", RegexOptions.Compiled)]
        private static partial Regex UsernameRegax();
        private static readonly Regex UsernameRegex = UsernameRegax();

        public LoginValidator()
        {
            RuleFor(x => x.UsernameOrEmail)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty().WithMessage(BaseMessages.UsernameRequired);

            When(x => !string.IsNullOrWhiteSpace(x.UsernameOrEmail) && x.UsernameOrEmail.Contains('@'), () =>
            {
                RuleFor(p => p.UsernameOrEmail)
                .EmailAddress()
                .WithMessage(BaseMessages.EmailInvalid);
            })
            .Otherwise(() =>
            {
                RuleFor(p => p.UsernameOrEmail)
                .Matches(UsernameRegex)
                .WithMessage(BaseMessages.UsernameInvalid);
            });

            RuleFor(p => p.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage(AuthenticationMessages.PasswordRequired)
                .MinimumLength(4)
                .WithMessage(AuthenticationMessages.PasswordMinLength);
        }
    }
}
