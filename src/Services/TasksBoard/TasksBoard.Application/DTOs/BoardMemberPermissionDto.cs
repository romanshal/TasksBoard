namespace TasksBoard.Application.DTOs
{
    public class BoardMemberPermissionDto
    {
        public Guid BoardMemberId { get; set; }
        public required Guid BoardPermissionId { get; set; }
        public required string BoardPermissionName { get; set; }
        public required string BoardPermissionDescription { get; set; }
    }
}
