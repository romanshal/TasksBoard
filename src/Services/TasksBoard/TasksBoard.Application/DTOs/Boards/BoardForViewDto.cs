namespace TasksBoard.Application.DTOs.Boards
{
    public class BoardForViewDto : BaseDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string[] Tags { get; set; } = [];
        public int MemberCount { get; set; }
        public bool IsMember { get; set; }
        public bool IsPublic { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageExtension { get; set; }
    }
}
