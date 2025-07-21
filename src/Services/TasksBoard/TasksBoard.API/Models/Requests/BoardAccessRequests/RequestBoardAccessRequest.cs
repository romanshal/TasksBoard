namespace TasksBoard.API.Models.Requests.BoardAccessRequests
{
    public record RequestBoardAccessRequest
    {
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required string AccountEmail { get; set; }
    }
}
