namespace TasksBoard.Application.DTOs
{
    public class BoardDto : BaseDto
    {
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string[] Tags { get; set; }
        public bool Public { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageExtension { get; set; }
        public IEnumerable<BoardMemberDto> Members { get; set; }
        public IEnumerable<BoardAccessRequestDto> AccessRequests { get; set; }
        public IEnumerable<BoardInviteRequestDto> InviteRequests { get; set; }
    }
}
