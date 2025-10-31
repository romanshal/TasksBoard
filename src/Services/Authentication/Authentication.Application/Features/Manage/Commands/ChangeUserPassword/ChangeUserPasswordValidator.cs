using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Manage.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(BaseMessages.UserIdRequired);

            RuleFor(p => p.CurrentPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(ManageMessages.CurrentPasswordRequired);

            RuleFor(p => p.NewPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage(ManageMessages.NewPasswordRequired);
        }
    }
}
