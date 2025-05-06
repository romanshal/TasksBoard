namespace Chat.Application.Models.Requests
{
    public class CreateBoardMessageRequest
    {
        public Guid MemberId { get; set; }
        public Guid AccountId { get; set; }
        public required string MemberNickname { get; set; }
        public required string Message { get; set; }
    }
}
