using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace Authentication.Domain.Constants.TwoFactor
{
    public static class TwoFactorErrors
    {
        /// <summary>
        /// Authenticator uses TOTP on device.
        /// </summary>
        public static readonly Error UseTOTP = new(ErrorCodes.Invalid, "Authenticator uses TOTP on device.");

        /// <summary>
        /// No any phone numbers for account.
        /// </summary>
        public static readonly Error NoPhone = new(ErrorCodes.Invalid, "No any phone numbers for account.");

        /// <summary>
        /// Invalid two factor provider.
        /// </summary>
        public static readonly Error InvalidProvider = new(ErrorCodes.Invalid, "Invalid two factor provider.");

        /// <summary>
        /// Can't send two factor cdoe. Please try later.
        /// </summary>
        public static readonly Error CantSend = new(ErrorCodes.CantCreate, "Can't send two factor code. Please try later.");

        /// <summary>
        /// Can't authenticate. Please try later.
        /// </summary>
        public static readonly Error CantVerify = new(ErrorCodes.CantCreate, "Can't verify code. Please try later.");
    }
}
