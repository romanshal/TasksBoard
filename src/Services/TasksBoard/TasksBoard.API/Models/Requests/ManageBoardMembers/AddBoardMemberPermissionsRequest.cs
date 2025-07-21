namespace TasksBoard.API.Models.Requests.ManageBoardMembers
{
    public record AddBoardMemberPermissionsRequest
    {
        public required Guid MemberId { get; set; }
        public required Guid[] Permissions { get; set; }
    }
}
