namespace TasksBoard.API.Models.Requests.BoardAccessRequests
{
    public record RequestBoardAccessRequest
    {
        public required Guid AccountId { get; set; }
    }
}
