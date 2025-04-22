namespace TasksBoard.Application.DTOs
{
    public class BoardMemberDto : BaseDto
    {
        public Guid BoardId { get; set; }
        public Guid AccountId { get; set; }
        public bool IsOwner { get; set; }
        public required string Nickname { get; set; }
        public IEnumerable<BoardMemberPermissionDto> Permissions { get; set; }
    }
}
