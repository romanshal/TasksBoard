namespace TasksBoard.Application.DTOs
{
    public class BoardDto : BaseDto
    {
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string[] Tags { get; set; }
        public int MemberCount { get; set; }
        public bool IsMember { get; set; }
        public bool Public { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageExtension { get; set; } 
    }
}
