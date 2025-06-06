using Common.Blocks.Interfaces.UnitOfWorks;
using Notification.Domain.Interfaces.Repositories;

namespace Notification.Domain.Interfaces.UnitOfWorks
{
    public interface IUnitOfWork : IUnitOfWorkBase
    {
        IApplicationEventRepository GetApplicationEventRepository();
    }
}
