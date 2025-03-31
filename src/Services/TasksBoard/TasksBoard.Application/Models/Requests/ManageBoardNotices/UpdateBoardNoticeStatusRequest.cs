namespace TasksBoard.Application.Models.Requests.ManageBoardNotices
{
    public class UpdateBoardNoticeStatusRequest
    {
        public Guid NoticeId { get; set; }
        public bool Complete { get; set; }
    }
}
