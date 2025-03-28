namespace TasksBoard.Application.Models.Requests.ManageBoardNotices
{
    public class UpdateBoardNoticeRequest
    {
        public required Guid NoticeId { get; set; }
        public required string Definition { get; set; }
        public Guid NoticeStatusId { get; set; }
        public required string BackgroundColor { get; set; }
        public required string Rotation { get; set; }
    }
}
