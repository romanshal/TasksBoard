namespace Common.Blocks.Exceptions
{
    public class SignoutFaultedException : Exception
    {
        public SignoutFaultedException() : base($"Signout faulted.")
        {
        }

        public SignoutFaultedException(string message)
            : base(message)
        { }

        public SignoutFaultedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
