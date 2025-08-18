using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Search.Queries.SearchUsersByQuery
{
    public class SearchUsersByQueryQuery : IRequest<IEnumerable<UserInfoDto>>
    {
        public required string Query { get; set; }
    }
}
