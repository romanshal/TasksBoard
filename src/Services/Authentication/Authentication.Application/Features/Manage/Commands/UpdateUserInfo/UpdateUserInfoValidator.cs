using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserInfo
{
    public class UpdateUserInfoValidator : AbstractValidator<UpdateUserInfoCommand>
    {
        public UpdateUserInfoValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(BaseMessages.UserIdRequired);

            RuleFor(p => p.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(BaseMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(BaseMessages.EmailInvalid);

            RuleFor(p => p.Username)
                .NotNull()
                .NotEmpty()
                .WithMessage(BaseMessages.UserIdRequired);
        }
    }
}
