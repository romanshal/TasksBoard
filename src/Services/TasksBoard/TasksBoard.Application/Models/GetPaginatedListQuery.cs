using Common.Blocks.CQRS;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Models
{
    public record GetPaginatedListQuery<T> : IQuery<Result<PaginatedList<T>>> where T : BaseDto
    {
        public required int PageIndex { get; set; }
        public required int PageSize { get; set; }
    }
}
