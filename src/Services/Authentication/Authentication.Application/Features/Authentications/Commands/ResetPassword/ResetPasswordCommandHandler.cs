using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Authentication.Application.Features.Authentications.Commands.ResetPassword
{
    internal class ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<ResetPasswordCommandHandler> logger) : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<ResetPasswordCommandHandler> _logger = logger;

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return Result.Failure(AuthenticationErrors.UserNotFound());
            }

            string token;
            try
            {
                var bytes = WebEncoders.Base64UrlDecode(request.Token);
                token = Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid token format for user {userId}", request.UserId);
                return Result.Failure(ResetPasswordErrors.InvalidToken);
            }

            var identityResult = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToArray();
                _logger.LogError("Password reset failed for user {userId}. Errors: {errors}", request.UserId, string.Join("; ", errors));

                return Result.Failure(ResetPasswordErrors.CantReset);
            }

            _logger.LogInformation("Password reset succeeded for user {userId}", request.UserId);

            return Result.Success();
        }
    }
}
