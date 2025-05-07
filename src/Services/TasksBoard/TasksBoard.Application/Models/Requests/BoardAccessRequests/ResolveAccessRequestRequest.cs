namespace TasksBoard.Application.Models.Requests.BoardAccessRequests
{
    public class ResolveAccessRequestRequest
    {
        public required Guid RequestId { get; set; }
        public bool Decision { get; set; }
    }
}
