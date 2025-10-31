using Authentication.Application.Dtos;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace Authentication.Application.Features.Authentications.Commands.GenerateAuthenticatorSetup
{
    internal class GenerateAuthenticatorSetupCommandHandler(
        UserManager<ApplicationUser> userManager) : IRequestHandler<GenerateAuthenticatorSetupCommand, Result<AuthenticatorSetupDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Result<AuthenticatorSetupDto>> Handle(GenerateAuthenticatorSetupCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result.Failure<AuthenticatorSetupDto>(AuthenticationErrors.UserNotFound());
            }

            // Ensure user has an authenticator key. If not, ResetAuthenticatorKeyAsync creates one.
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            // ManualEntryKey is the base32 secret (uppercase with spaces friendly to manual entry)
            var manualKey = FormatKey(key);

            var email = user.Email!;
            var issuer = request.Issuer ?? "MyApp";
            var label = $"{issuer}:{email}";
            var uri = $"otpauth://totp/{UrlEncode(label)}?secret={key}&issuer={UrlEncode(issuer)}&digits=6";

            return new AuthenticatorSetupDto
            {
                ManualEntryKey = manualKey,
                QrCodeUri = uri
            };
        }

        private static string FormatKey(string unformattedKey)
        {
            // Identity returns a base32 value; present in groupings (e.g., 4 chars)
            const int chunkSize = 4;
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < unformattedKey.Length; i++)
            {
                if (i > 0 && i % chunkSize == 0) sb.Append(' ');
                sb.Append(char.ToUpperInvariant(unformattedKey[i]));
            }
            return sb.ToString();
        }

        private static string UrlEncode(string value) => HttpUtility.UrlEncode(value);
    }
}
