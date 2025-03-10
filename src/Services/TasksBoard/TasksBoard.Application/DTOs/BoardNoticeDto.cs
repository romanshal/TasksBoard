namespace TasksBoard.Application.DTOs
{
    public class BoardNoticeDto
    {
        public Guid Id { get; set; }
        public required Guid AuthorId { get; set; }
        public required Guid BoardId { get; set; }
        public required string Definition { get; set; }
        public required Guid NoticeStatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
