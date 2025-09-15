using Authentication.Application.Dtos;
using Common.Blocks.Models.DomainResults;
using Common.Cache.CQRS;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommand : ICachebleCommand<UserInfoDto, Result<UserInfoDto>>
    {
        public required Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}
