using Chat.Domain.Entities;
using Chat.Domain.Interfaces.Repositories;
using Chat.Domain.Interfaces.UnitOfWorks;
using Chat.Domain.ValueObjects;
using Chat.Infrastructure.Data.Contexts;
using Common.Blocks.UnitOfWorks;

namespace Chat.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        IServiceProvider serviceProvider) : UnitOfWorkBase<ChatDbContext>(serviceProvider), IUnitOfWork
    {
        public IBoardMessageRepository GetBoardMessagesRepository()
        {
            return base.Repository<BoardMessage, MessageId, IBoardMessageRepository>();
        }
    }
}
