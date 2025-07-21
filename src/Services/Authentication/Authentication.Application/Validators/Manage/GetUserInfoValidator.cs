using Authentication.Application.Features.Manage.Queries.GetUserInfo;
using Authentication.Domain.Constants.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Manage
{
    public class GetUserInfoValidator : AbstractValidator<GetUserInfoQuery>
    {
        public GetUserInfoValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(ManageMessages.UserIdRequired);
        }
    }
}
