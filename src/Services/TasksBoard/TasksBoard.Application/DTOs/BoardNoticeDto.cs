namespace TasksBoard.Application.DTOs
{
    public class BoardNoticeDto : BaseDto
    {
        public required Guid AuthorId { get; set; }
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required string Definition { get; set; }
        public required string BackgroundColor { get; set; }
        public required string Rotation { get; set; }
        public required bool Completed { get; set; }
    }
}
