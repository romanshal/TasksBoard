namespace TasksBoard.API.Models.Requests.ManageBoardNotices
{
    public record CreateBoardNoticeRequest
    {
        public required Guid AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public required string Definition { get; set; }
        public required string BackgroundColor { get; set; }
        public required string Rotation { get; set; }
    }
}
