using Chat.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Features.BoardMessages.Commands.CreateBoardMessage
{
    public class CreateBoardMessageCommand : IRequest<BoardMessageDto>
    {
        public required Guid BoardId { get; set; }
        public required Guid MemberId { get; set; }
        public required Guid AccountId { get; set; }
        public required string MemberNickname { get; set; }
        public required string Message { get; set; }
    }
}
