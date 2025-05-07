namespace TasksBoard.Application.Models.Requests.ManageBoardMembers
{
    public class AddBoardMemberRequest
    {
        public required Guid AccountId { get; set; }
        public required string Nickname { get; set; }
    }
}
