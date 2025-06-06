namespace TasksBoard.Application.Models.Requests.BoardInviteRequests
{
    public class ResolveInviteRequestRequest
    {
        public required Guid RequestId { get; set; }
        public required bool Decision { get; set; }
    }
}
