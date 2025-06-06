using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Manage.Queries.GetUserImage
{
    public class GetUserImageQuery : IRequest<UserImageDto>
    {
        public required Guid UserId { get; set; }
    }
}
