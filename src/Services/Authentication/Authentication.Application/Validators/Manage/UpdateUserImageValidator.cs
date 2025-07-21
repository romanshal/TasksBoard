using Authentication.Application.Features.Manage.Commands.UpdateUserImage;
using Authentication.Domain.Constants.Messages;
using FluentValidation;
using System.Data;

namespace Authentication.Application.Validators.Manage
{
    public class UpdateUserImageValidator : AbstractValidator<UpdateUserImageCommand>
    {
        public UpdateUserImageValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(ManageMessages.UserIdRequired);

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
