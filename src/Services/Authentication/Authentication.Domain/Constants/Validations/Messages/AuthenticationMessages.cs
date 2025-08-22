namespace Authentication.Domain.Constants.Validations.Messages
{
    public class AuthenticationMessages : BaseMessages
    {
        public const string PasswordRequired = "Password is required.";
        public const string PasswordMinLength = "Password length must be greater than 4.";
        public const string RefreshTokenRequired = "Refresh token is required.";

    }
}
