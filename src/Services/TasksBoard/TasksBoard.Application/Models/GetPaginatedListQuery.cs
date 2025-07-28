using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Models
{
    public record GetPaginatedListQuery<T> : IRequest<Result<PaginatedList<T>>> where T : BaseDto
    {
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
