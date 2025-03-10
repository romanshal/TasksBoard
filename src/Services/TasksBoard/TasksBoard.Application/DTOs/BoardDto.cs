namespace TasksBoard.Application.DTOs
{
    public class BoardDto
    {
        public required Guid Id { get; set; }
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
