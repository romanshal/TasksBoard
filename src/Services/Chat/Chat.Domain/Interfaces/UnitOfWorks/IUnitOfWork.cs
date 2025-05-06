using Chat.Domain.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;

namespace Chat.Domain.Interfaces.UnitOfWorks
{
    public interface IUnitOfWork : IUnitOfWorkBase
    {
        public IBoardMessageRepository GetBoardMessagesRepository();
    }
}
