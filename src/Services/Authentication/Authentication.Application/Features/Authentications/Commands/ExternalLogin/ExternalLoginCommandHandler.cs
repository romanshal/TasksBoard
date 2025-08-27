using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using Common.Blocks.Models.DomainResults;
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
            var providers = await _signInManager.GetExternalAuthenticationSchemesAsync();

            if (string.IsNullOrWhiteSpace(request.Provider))
            {
                _logger.LogWarning("Invalid prodiver.");
                throw new NotFoundException("Provider is required.");
            }

            var provider  = providers.FirstOrDefault(p => string.Equals(p.Name, request.Provider, StringComparison.OrdinalIgnoreCase));
            if(provider is null)
            {
                _logger.LogWarning("Unsupported provider: {povider}", request.Provider);
                throw new NotFoundException($"Unsupported provider: {request.Provider}");
            }

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                provider.Name,
                request.RedirectUrl
            );

            return properties;
        }
    }
}
