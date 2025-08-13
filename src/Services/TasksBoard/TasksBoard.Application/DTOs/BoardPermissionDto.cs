namespace TasksBoard.Application.DTOs
{
    public class BoardPermissionDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public required int AccessLevel { get; set; }
    }
}
