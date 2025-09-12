using Chat.Domain.Entities;
using Chat.Domain.Interfaces.Repositories;
using Chat.Domain.Interfaces.UnitOfWorks;
using Chat.Domain.ValueObjects;
using Chat.Infrastructure.Data.Contexts;
using Chat.Infrastructure.Repositories;
using Common.Blocks.Repositories;
using Common.Blocks.UnitOfWorks;
using Microsoft.Extensions.Logging;

namespace Chat.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        ChatDbContext context,
        ILoggerFactory loggerFactory) : UnitOfWorkBase(context, loggerFactory), IUnitOfWork
    {
        private readonly ChatDbContext _context = context;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;

        public IBoardMessageRepository GetBoardMessagesRepository()
        {
            var type = typeof(BoardMessage);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<BoardMessage, MessageId>))
            {
                var repositoryInstance = new BoardMessageRepository(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IBoardMessageRepository)value;
        }
    }
}
