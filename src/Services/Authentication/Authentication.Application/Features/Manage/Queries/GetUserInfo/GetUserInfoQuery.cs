using Authentication.Application.Dtos;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Manage.Queries.GetUserInfo
{
    public class GetUserInfoQuery : IRequest<Result<UserInfoDto>>
    {
        public Guid UserId { get; set; }
    }
}
