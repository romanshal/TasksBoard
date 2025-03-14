namespace TasksBoard.Application.Models.Requests.BoardNotices
{
    public class CreateBoardNoticeRequest
    {
        public required Guid AuthorId { get; set; }
        public required string Definition { get; set; }
        public required Guid NoticeStatusId { get; set; }
    }
}
