using Authentication.Application.Features.Manage.Commands.UpdateUserInfo;
using Authentication.Domain.Constants.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Manage
{
    public class UpdateUserInfoValidator : AbstractValidator<UpdateUserInfoCommand> 
    {
        public UpdateUserInfoValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(ManageMessages.UserIdRequired);

            RuleFor(p => p.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(ManageMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(ManageMessages.EmailInvalid);

            RuleFor(p => p.Username)
                .NotNull()
                .NotEmpty()
                .WithMessage(ManageMessages.UserIdRequired);
        }
    }
}
