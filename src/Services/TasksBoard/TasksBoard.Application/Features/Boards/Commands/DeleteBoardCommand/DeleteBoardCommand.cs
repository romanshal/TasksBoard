using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksBoard.Application.Features.Boards.Commands.DeleteBoardCommand
{
    public class DeleteBoardCommand : IRequest<Unit>
    {
        public required Guid Id { get; set; }
    }
}
