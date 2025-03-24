using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.ExternalLogin
{
    public class ExternalLoginCommand : IRequest<AuthenticationDto>
    {
        public string? Provider { get; set; }
        public string? TokenId { get; set; }
    }
}
