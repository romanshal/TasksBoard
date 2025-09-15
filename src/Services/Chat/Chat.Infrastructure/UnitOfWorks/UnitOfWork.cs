using Chat.Domain.Entities;
using Chat.Domain.Interfaces.Repositories;
using Chat.Domain.Interfaces.UnitOfWorks;
using Chat.Domain.ValueObjects;
using Chat.Infrastructure.Data.Contexts;
using Common.Blocks.Repositories;
using Common.Blocks.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        IServiceProvider serviceProvider) : UnitOfWorkBase<ChatDbContext>(serviceProvider), IUnitOfWork
    {
        public IBoardMessageRepository GetBoardMessagesRepository()
        {
            var type = typeof(BoardMessage);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardMessage, MessageId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IBoardMessageRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardMessageRepository)value;
        }
    }
}
