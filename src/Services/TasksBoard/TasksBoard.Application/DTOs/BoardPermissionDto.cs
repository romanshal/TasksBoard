namespace TasksBoard.Application.DTOs
{
    public class BoardPermissionDto : BaseDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public required int AccessLevel { get; set; }
    }
}
