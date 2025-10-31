using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace Authentication.Domain.Constants.AuthenticationErrors
{
    public static class ResetPasswordErrors
    {
        /// <summary>
        /// Invalid token.
        /// </summary>
        public static readonly Error InvalidToken = new(ErrorCodes.Invalid, "Invalid token.");

        /// <summary>
        /// Can't update password. Please try later.
        /// </summary>
        public static readonly Error CantReset = new(ErrorCodes.Invalid, "Can't update password. Please try later.");
    }
}
