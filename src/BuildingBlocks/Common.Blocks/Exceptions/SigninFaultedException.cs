namespace Common.Blocks.Exceptions
{
    public class SigninFaultedException : Exception
    {
        public SigninFaultedException() : base($"The username or password you entered is incorrect. Please try again.")
        {
        }

        public SigninFaultedException(string message)
            : base(message)
        { }

        public SigninFaultedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
