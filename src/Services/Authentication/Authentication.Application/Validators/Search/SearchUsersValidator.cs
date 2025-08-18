using Authentication.Application.Features.Search.Queries.SearchUsersByQuery;
using Authentication.Domain.Constants.Messages;
using FluentValidation;

namespace Authentication.Application.Validators.Search
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
