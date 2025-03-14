namespace TasksBoard.Application.Models.Requests.BoardNotices
{
    public class UpdateBoardNoticeRequest
    {
        public required Guid NoticeId { get; set; }
        public required string Definition { get; set; }
        public Guid NoticeStatusId { get; set; }
    }
}
