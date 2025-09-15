using Authentication.Application.Dtos;
using Common.Blocks.Models.DomainResults;
using Common.Cache.CQRS;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserImage
{
    public class UpdateUserImageCommand : ICachebleCommand<UserImageDto, Result>
    {
        public required Guid Id { get; set; }
        public required byte[] Image { get; set; }
        public required string ImageExtension { get; set; }
    }
}
