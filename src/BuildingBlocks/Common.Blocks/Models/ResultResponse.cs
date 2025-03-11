namespace Common.Blocks.Models
{
    public class ResultResponse<T> : Response
    {
        public T Result { get; set; }

        public ResultResponse(T result) : base()
        {
            Result = result;
        }

        public ResultResponse(T result, string? description, bool isError) : base(description, isError)
        {
            Result = result;
        }
    }
}
