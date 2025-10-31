namespace Authentication.Domain.Constants.Validations.Messages
{
    public class AuthenticationMessages : BaseMessages
    {
        /// <summary>
        /// Password is required.
        /// </summary>
        public const string PasswordRequired = "Password is required.";

        /// <summary>
        /// Password length must be greater than 4.
        /// </summary>
        public const string PasswordMinLength = "Password length must be greater than 4.";

        /// <summary>
        /// Refresh token is required.
        /// </summary>
        public const string RefreshTokenRequired = "Refresh token is required.";

        /// <summary>
        /// Reset token is required.s
        /// </summary>
        public const string ResetTokenRequired = "Reset token is required.";

    }
}
