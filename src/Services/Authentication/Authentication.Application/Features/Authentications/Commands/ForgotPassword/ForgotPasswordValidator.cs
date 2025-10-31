using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Authentications.Commands.ForgotPassword
{
    internal sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(BaseMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(BaseMessages.EmailInvalid);
        }
    }
}
