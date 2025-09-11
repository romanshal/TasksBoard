using Newtonsoft.Json;

namespace Common.Blocks.Models.ApiResponses
{
    public class ApiResponse<T> : ApiResponse
    {
        private readonly T? _result;

        [JsonConstructor]
        protected internal ApiResponse(T? result, string? description = default, bool isError = false) : base(description, isError) =>
            _result = result;

        //TODO: return this and change logic
        //[NotNull]
        //public T Result => _result! ?? throw new InvalidOperationException("Result has no value.");

        public T? Result => _result;

        public static implicit operator ApiResponse<T>(T? value) => Create(value);
    }
}
