using Newtonsoft.Json;
using System.Text.Json;

namespace Common.Blocks.Models.ApiResponses
{
    public class ApiResponse
    {
        public string? Description { get; }
        public bool IsError { get; }

        [JsonConstructor]
        protected ApiResponse(string? description = default, bool isError = false)
        {
            Description = description;
            IsError = isError;
        }

        public static ApiResponse Success() => new(isError: false);

        public static ApiResponse Error(string description) => new(description, true);

        public static ApiResponse<T> Success<T>(T result) => new(result, isError: false);

        public static ApiResponse<T> Error<T>(string description) => new(default, description, true);

        public static ApiResponse<T> Create<T>(T? value) =>
            value is not null ? Success(value) : Error<T>(string.Empty);

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
