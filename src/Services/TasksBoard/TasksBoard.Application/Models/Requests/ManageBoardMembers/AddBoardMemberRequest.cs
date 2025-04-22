namespace TasksBoard.Application.Models.Requests.ManageBoardMembers
{
    public class AddBoardMemberRequest
    {
        public required Guid UserId { get; set; }
        public required string Nickname { get; set; }
        public Guid[] Permissions { get; set; } = [];
    }
}
