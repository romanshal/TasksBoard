using Authentication.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace Authentication.Application.Features.Authentications.Commands.ExternalLogin
{
    public record ExternalLoginCommand : IRequest<AuthenticationProperties>
    {
        public string? Provider { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
