namespace Common.Blocks.Exceptons
{
    public class NotFoundException<T> : Exception where T : class
    {
        public NotFoundException() : base($"Entity '{nameof(T)}' was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        { }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
