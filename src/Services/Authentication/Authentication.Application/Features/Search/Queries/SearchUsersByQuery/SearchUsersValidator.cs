using Authentication.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Authentication.Application.Features.Search.Queries.SearchUsersByQuery
{
    public class SearchUsersValidator : AbstractValidator<SearchUsersByQueryQuery>
    {
        public SearchUsersValidator()
        {
            RuleFor(p => p.Query)
                .NotNull()
                .NotEmpty()
                .WithMessage(SearchMessages.QueryRequired);
        }
    }
}
