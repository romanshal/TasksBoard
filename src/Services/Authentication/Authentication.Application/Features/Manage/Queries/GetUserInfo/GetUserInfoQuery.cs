using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Manage.Queries.GetUserInfo
{
    public class GetUserInfoQuery : IRequest<UserInfoDto>
    {
        public Guid UserId { get; set; }
    }
}
