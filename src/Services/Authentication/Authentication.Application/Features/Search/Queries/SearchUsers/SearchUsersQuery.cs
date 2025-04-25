using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Search.Queries.SearchUsers
{
    public class SearchUsersQuery : IRequest<IEnumerable<UserInfoDto>>
    {
        public required string Query { get; set; }
    }
}
