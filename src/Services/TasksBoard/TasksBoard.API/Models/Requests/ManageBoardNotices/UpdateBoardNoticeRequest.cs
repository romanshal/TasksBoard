namespace TasksBoard.API.Models.Requests.ManageBoardNotices
{
    public record UpdateBoardNoticeRequest
    {
        public required Guid NoticeId { get; set; }
        public required string Definition { get; set; }
        public required string BackgroundColor { get; set; }
        public required string Rotation { get; set; }
    }
}
