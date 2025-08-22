using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.ExternalLogin
{
    public class ExternalLoginCommandHandler(
        SignInManager<ApplicationUser> signInManager,
        ILogger<ExternalLoginCommandHandler> logger) : IRequestHandler<ExternalLoginCommand, AuthenticationProperties>
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ILogger<ExternalLoginCommandHandler> _logger = logger;

        public async Task<AuthenticationProperties> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            var providers = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Select(p => p.Name).ToList();

            if (string.IsNullOrWhiteSpace(request.Provider) || !providers.Contains(request.Provider, StringComparer.OrdinalIgnoreCase))
                throw new NotFoundException("Unsupported provider.");

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(request.Provider, request.RedirectUrl);

            return properties;
        }
    }
}
