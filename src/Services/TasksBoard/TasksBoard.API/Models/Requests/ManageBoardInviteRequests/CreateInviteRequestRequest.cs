namespace TasksBoard.API.Models.Requests.ManageBoardInviteRequests
{
    public record CreateInviteRequestRequest
    {
        public required Guid FromAccountId { get; set; }
        public required Guid ToAccountId { get; set; }
    }
}
