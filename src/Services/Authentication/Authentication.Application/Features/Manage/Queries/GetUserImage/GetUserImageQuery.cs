using Authentication.Application.Dtos;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Manage.Queries.GetUserImage
{
    public class GetUserImageQuery : IRequest<Result<UserImageDto>>
    {
        public required Guid UserId { get; set; }
    }
}
