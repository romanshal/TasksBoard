namespace Chat.API.Models.Requests
{
    public record CreateBoardMessageRequest
    {
        public Guid MemberId { get; set; }
        public Guid AccountId { get; set; }
        public required string Message { get; set; }
    }
}
