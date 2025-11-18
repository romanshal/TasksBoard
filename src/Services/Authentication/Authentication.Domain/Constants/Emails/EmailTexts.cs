namespace Authentication.Domain.Constants.Emails
{
    public static class EmailTexts
    {
        public static string ResetPassword(string clientUrl, Guid userId, string token) => $"Reset password <a href=\"{clientUrl}?u={userId}&t={token}\">link</a>.";
        public static string ConfirmEmail(string clientUrl, Guid userId, string token) => $"Confirm email <a href=\"{clientUrl}?u={userId}&t={token}\">link</a>.";
        public static string TwoFactor(string clientUrl, Guid userId, string token) => $"Tow factor <a href=\"{clientUrl}?u={userId}&t={token}\">link</a>.";
    }
}
