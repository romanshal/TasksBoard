using Authentication.Application.Features.Manage.Queries.GetUserInfo;
using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Manage
{
    public class GetUserInfoValidator : AbstractValidator<GetUserInfoQuery>
    {
        public GetUserInfoValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(ManageMessages.UserIdRequired);
        }
    }
}
