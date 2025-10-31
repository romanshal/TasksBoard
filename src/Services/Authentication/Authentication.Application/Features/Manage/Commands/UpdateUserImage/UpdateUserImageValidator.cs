using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserImage
{
    public class UpdateUserImageValidator : AbstractValidator<UpdateUserImageCommand>
    {
        public UpdateUserImageValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(BaseMessages.UserIdRequired);

            RuleFor(p => p.Image)
                .NotNull()
                .NotEmpty()
                .WithMessage(ManageMessages.ImageRequired);

            RuleFor(p => p.ImageExtension)
                .NotNull()
                .NotEmpty()
                .WithMessage(ManageMessages.ImageRequired);
        }
    }
}
