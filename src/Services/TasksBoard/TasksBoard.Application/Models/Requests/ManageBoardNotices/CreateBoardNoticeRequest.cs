namespace TasksBoard.Application.Models.Requests.ManageBoardNotices
{
    public class CreateBoardNoticeRequest
    {
        public required Guid AuthorId { get; set; }
        public required string Definition { get; set; }
        public required Guid NoticeStatusId { get; set; }
    }
}
