namespace TasksBoard.API.Models.Requests.Boards
{
    public record CreateBoardRequest
    {
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public required string OwnerNickname { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public bool Public { get; set; }
        public IFormFile? Image { get; set; }
    }
}
