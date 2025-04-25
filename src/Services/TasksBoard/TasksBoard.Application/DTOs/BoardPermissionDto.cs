namespace TasksBoard.Application.DTOs
{
    public class BoardPermissionDto : BaseDto
    {
        public string Name { get; set; }
        public string Desctiption { get; set; }
        public required int AccessLevel { get; set; }
    }
}
