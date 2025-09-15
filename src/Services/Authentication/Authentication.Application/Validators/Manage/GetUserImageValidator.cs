using Authentication.Application.Features.Manage.Queries.GetUserImage;
using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Manage
{
    public class GetUserImageValidator : AbstractValidator<GetUserImageQuery>
    {
        public GetUserImageValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(ManageMessages.UserIdRequired);
        }
    }
}
