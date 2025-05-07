namespace TasksBoard.Application.Models.Requests.BoardAccessRequests
{
    public class RequestBoardAccessRequest
    {
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required string AccountEmail { get; set; }
    }
}
