using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Authentications.Commands.ResetPassword
{
    internal sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .NotEqual(Guid.Empty)
                .WithMessage(BaseMessages.UserIdRequired);

            RuleFor(x => x.Token)
                .NotEmpty()
                .NotNull()
                .WithMessage(AuthenticationMessages.ResetTokenRequired);

            RuleFor(p => p.NewPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(AuthenticationMessages.PasswordRequired)
                .MinimumLength(4)
                .WithMessage(AuthenticationMessages.PasswordMinLength);
        }
    }
}
