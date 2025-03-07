using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksBoard.Application.Features.Boards.Commands.UpdateBoard
{
    public class UpdateBoardCommand : IRequest<Guid>
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
    }
}
