namespace Authentication.Infrastructure.Options
{
    internal class FrontendUrlsOption
    {
        public required string ConfirmEmail { get; set; }
        public required string ResetPassword { get; set; }
        public required string TwoFactor { get; set; }
    }
}
