using Common.Blocks.Models;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Models
{
    public record GetPaginatedListQuery<T> : IRequest<PaginatedList<T>> where T : BaseDto
    {
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
