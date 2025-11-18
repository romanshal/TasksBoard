namespace Authentication.Infrastructure.Options
{
    internal class ClientUrlsOption
    {
        public required string ConfirmEmail { get; set; }
        public required string ResetPassword { get; set; }
        public required string TwoFactor { get; set; }
    }
}
