namespace TasksBoard.API.Models.Requests.ManageBoardAccessRequests
{
    public record ResolveAccessRequestRequest
    {
        public required Guid RequestId { get; set; }
        public bool Decision { get; set; }
    }
}
