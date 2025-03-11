namespace TasksBoard.Application.DTOs
{
    public class BoardDto : BaseDto
    {
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
