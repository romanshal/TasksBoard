namespace TasksBoard.API.Models.Requests.BoardInviteRequests
{
    public record ResolveInviteRequestRequest
    {
        public required Guid RequestId { get; set; }
        public required bool Decision { get; set; }
    }
}
