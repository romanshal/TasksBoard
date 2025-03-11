using System.Text.Json;

namespace Common.Blocks.Models
{
    public class Response(string? description = default, bool isError = false)
    {
        public string? Description { get; set; } = description;
        public bool IsError { get; set; } = isError;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
