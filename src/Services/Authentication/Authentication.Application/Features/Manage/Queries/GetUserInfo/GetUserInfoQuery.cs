using Authentication.Application.Dtos;
using Common.Blocks.Models.DomainResults;
using Common.Cache.CQRS;

namespace Authentication.Application.Features.Manage.Queries.GetUserInfo
{
    public class GetUserInfoQuery : ICachebleQuery<UserInfoDto, Result<UserInfoDto>>
    {
        public Guid Id { get; set; }
    }
}
