using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Common.Blocks.CQRS
{
    public interface IQuery<TResponse> : IRequest<TResponse> where TResponse : Result
    {
    }
}
