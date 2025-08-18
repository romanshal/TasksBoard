using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Search.Queries.SearchUsersById
{
    public class SearchUsersByIdQuery : IRequest<IEnumerable<UserInfoDto>>
    {
        public required IEnumerable<Guid> UserIds { get; set; }
    }
}
