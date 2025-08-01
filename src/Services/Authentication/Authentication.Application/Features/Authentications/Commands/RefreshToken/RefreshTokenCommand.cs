﻿using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.RefreshToken
{
    public record RefreshTokenCommand : IRequest<AuthenticationDto>
    {
        public required Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
