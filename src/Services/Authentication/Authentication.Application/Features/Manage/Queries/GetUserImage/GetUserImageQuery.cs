using Authentication.Application.Dtos;
using Common.Blocks.Models.DomainResults;
using Common.Cache.CQRS;

namespace Authentication.Application.Features.Manage.Queries.GetUserImage
{
    public class GetUserImageQuery : ICachebleQuery<UserImageDto, Result<UserImageDto>>
    {
        public required Guid Id { get; set; }
    }
}
