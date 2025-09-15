using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Common.Blocks.CQRS
{
    public interface ICommand<TResponse> : IRequest<TResponse> where TResponse : Result
    {
    }
}
