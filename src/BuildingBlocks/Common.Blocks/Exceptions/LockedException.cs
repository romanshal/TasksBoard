namespace Common.Blocks.Exceptions
{
    public class LockedException : Exception
    {
        public LockedException() : base($"Your account is temporarily locked. Please contact support for assistance.")
        {
        }

        public LockedException(string message)
            : base(message)
        { }

        public LockedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
