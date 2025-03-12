namespace Common.Blocks.Exceptions
{
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException() : base($"Entity is already exist.")
        {
        }

        public AlreadyExistException(string message)
            : base(message)
        { }

        public AlreadyExistException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
