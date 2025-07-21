namespace TasksBoard.API.Models.Requests.ManageBoardMembers
{
    public record AddBoardMemberRequest
    {
        public required Guid AccountId { get; set; }
        public required string Nickname { get; set; }
    }
}
