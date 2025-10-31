namespace Authentication.Domain.Constants.TwoFactor
{
    public static class TwoFactorSenderMessages
    {
        public static readonly string Title = $"Two factor authentication.";
        public static string Body(string token) => $"Your login code is: {token}";
    }
}
