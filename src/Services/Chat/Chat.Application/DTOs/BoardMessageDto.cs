namespace Chat.Application.DTOs
{
    public class BoardMessageDto
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Guid AccountId { get; set; }
        public required string MemberNickname { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
