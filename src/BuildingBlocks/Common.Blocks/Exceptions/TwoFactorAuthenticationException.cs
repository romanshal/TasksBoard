namespace Common.Blocks.Exceptions
{
    public class TwoFactorAuthenticationException : Exception
    {
        public TwoFactorAuthenticationException() : base($"Two factor authentication exception.")
        {
        }

        public TwoFactorAuthenticationException(string message)
            : base(message)
        { }

        public TwoFactorAuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
