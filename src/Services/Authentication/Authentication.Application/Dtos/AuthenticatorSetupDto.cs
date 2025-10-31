namespace Authentication.Application.Dtos
{
    public class AuthenticatorSetupDto
    {
        public required string ManualEntryKey { get; init; } // Base32 key user types into authenticator
        public required string QrCodeUri { get; init; } // otpauth:// URI that can be encoded into a QR image on frontend
    }
}
