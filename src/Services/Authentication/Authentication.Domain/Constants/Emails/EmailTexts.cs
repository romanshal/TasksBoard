namespace Authentication.Domain.Constants.Emails
{
    public static class EmailTexts
    {
        public static string ResetPassword(string token) => $"Reset password link: {token}";
        public static string ConfirmEmail(string token) => $"Confirm email link: {token}";
        public static string TwoFactor(string token) => $"Tow factor link: {token}";
    }
}
