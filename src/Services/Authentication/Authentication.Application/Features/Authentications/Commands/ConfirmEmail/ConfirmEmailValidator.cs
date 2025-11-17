using FluentValidation;

namespace Authentication.Application.Features.Authentications.Commands.ConfirmEmail
{
    internal sealed class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .NotEqual(Guid.Empty);

            RuleFor(x => x.Token)
                .NotNull()
                .NotEmpty();
        }
    }
}
