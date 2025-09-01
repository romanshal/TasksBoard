namespace TasksBoard.Application.DTOs.Boards
{
    public class BoardFullDto : BoardDto
    {
        public IEnumerable<BoardMemberDto> Members { get; set; } = [];
        public IEnumerable<BoardAccessRequestDto> AccessRequests { get; set; } = [];
        public IEnumerable<BoardInviteRequestDto> InviteRequests { get; set; } = [];
    }
}
