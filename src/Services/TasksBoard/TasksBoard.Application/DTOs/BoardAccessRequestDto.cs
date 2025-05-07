namespace TasksBoard.Application.DTOs
{
    public class BoardAccessRequestDto : BaseDto
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required string AccountEmail { get; set; }
    }
}
