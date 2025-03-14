namespace TasksBoard.Application.Models.Requests.ManageBoards
{
    public class UpdateBoardRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
