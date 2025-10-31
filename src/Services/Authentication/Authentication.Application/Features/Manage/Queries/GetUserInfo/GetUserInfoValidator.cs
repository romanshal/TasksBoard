using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Manage.Queries.GetUserInfo
{
    public class GetUserInfoValidator : AbstractValidator<GetUserInfoQuery>
    {
        public GetUserInfoValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(BaseMessages.UserIdRequired);
        }
    }
}
