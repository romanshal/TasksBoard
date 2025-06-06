namespace Chat.Application.Models.Requests
{
    public class UpdateBoardMessageRequest
    {
        public Guid BoardMessageId { get; set; }
        public required string Message { get; set; }
    }
}
