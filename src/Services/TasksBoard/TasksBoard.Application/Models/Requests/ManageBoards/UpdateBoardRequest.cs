using Microsoft.AspNetCore.Http;

namespace TasksBoard.Application.Models.Requests.ManageBoards
{
    public class UpdateBoardRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string[] Tags { get; set; }
        public bool Public { get; set; }
        public IFormFile? Image { get; set; }
    }
}
