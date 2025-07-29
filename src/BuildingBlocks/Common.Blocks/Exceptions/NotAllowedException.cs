namespace Common.Blocks.Exceptions
{
    public class NotAllowedException : Exception
    {
        public NotAllowedException() : base($"Signin is not allowed.")
        {
        }

        public NotAllowedException(string message)
            : base(message)
        { }

        public NotAllowedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
