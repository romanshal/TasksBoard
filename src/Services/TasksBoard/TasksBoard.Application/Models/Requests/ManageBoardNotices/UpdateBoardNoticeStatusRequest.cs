namespace TasksBoard.Application.Models.Requests.ManageBoardNotices
{
    public class UpdateBoardNoticeStatusRequest
    {
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required Guid NoticeId { get; set; }
        public bool Complete { get; set; }
    }
}
