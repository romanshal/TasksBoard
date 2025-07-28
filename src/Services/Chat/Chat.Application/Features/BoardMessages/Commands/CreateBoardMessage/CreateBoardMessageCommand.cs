using Chat.Application.DTOs;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Chat.Application.Features.BoardMessages.Commands.CreateBoardMessage
{
    public class CreateBoardMessageCommand : IRequest<Result<BoardMessageDto>>
    {
        public required Guid BoardId { get; set; }
        public required Guid MemberId { get; set; }
        public required Guid AccountId { get; set; }
        public required string MemberNickname { get; set; }
        public required string Message { get; set; }
    }
}
