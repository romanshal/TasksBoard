namespace TasksBoard.Application.Models.Requests.ManageBoardMembers
{
    public class AddBoardMemberPermissionsRequest
    {
        public required Guid MemberId { get; set; }
        public required Guid[] Permissions { get; set; }
    }
}
