using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Manage.Queries.GetUserImage
{
    public class GetUserImageValidator : AbstractValidator<GetUserImageQuery>
    {
        public GetUserImageValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(BaseMessages.UserIdRequired);
        }
    }
}
