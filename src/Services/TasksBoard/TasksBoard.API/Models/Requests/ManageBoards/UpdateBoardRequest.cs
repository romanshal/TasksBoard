namespace TasksBoard.API.Models.Requests.ManageBoards
{
    public record UpdateBoardRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string[] Tags { get; set; }
        public bool Public { get; set; }
        public IFormFile? Image { get; set; }
    }
}
