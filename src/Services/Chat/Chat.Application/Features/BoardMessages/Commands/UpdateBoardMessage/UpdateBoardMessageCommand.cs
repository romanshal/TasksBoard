using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage
{
    public class UpdateBoardMessageCommand : IRequest<Guid>
    {
        public required Guid BoardId { get; set; }
        public required Guid BoardMessageId { get; set; }
        public required string Message { get; set; }
    }
}
