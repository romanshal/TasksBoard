using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserImage
{
    public class UpdateUserImageCommand : IRequest<Guid>
    {
        public required Guid UserId { get; set; }
        public required byte[] Image { get; set; }
        public required string ImageExtension { get; set; }
    }
}
