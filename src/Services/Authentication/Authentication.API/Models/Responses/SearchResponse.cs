namespace Authentication.API.Models.Responses
{
    public class SearchResponse
    {
        public required Guid UserId { get; set; }
        public required string Username { get; set; }
    }
}
