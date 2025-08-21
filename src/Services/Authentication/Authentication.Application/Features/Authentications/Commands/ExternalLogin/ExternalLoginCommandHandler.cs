using Authentication.Application.Dtos;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.ExternalLogin
{
    public class ExternalLoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<ExternalLoginCommandHandler> logger) : IRequestHandler<ExternalLoginCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<ExternalLoginCommandHandler> _logger = logger;

        public async Task<AuthenticationDto> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            

            return new AuthenticationDto
            {
                AccessToken = "",
                RefreshToken = "",
                UserId = Guid.Empty
            };
        }
    }
}
