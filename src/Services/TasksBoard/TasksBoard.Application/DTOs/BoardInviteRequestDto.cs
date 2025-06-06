namespace TasksBoard.Application.DTOs
{
    public class BoardInviteRequestDto : BaseDto
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid FromAccountId { get; set; }
        public required string FromAccountName { get; set; }
        public required Guid ToAccountId { get; set; }
        public required string ToAccountName { get; set; }
        public required string ToAccountEmail { get; set; }
    }
}
