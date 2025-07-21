namespace Chat.API.Models.Requests
{
    public record UpdateBoardMessageRequest
    {
        public Guid BoardMessageId { get; set; }
        public required string Message { get; set; }
    }
}
