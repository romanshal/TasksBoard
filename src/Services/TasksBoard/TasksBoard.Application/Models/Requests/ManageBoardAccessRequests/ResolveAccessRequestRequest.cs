namespace TasksBoard.Application.Models.Requests.ManageBoardAccessRequests
{
    public class ResolveAccessRequestRequest
    {
        public required Guid RequestId { get; set; }
        public bool Decision { get; set; }
    }
}
