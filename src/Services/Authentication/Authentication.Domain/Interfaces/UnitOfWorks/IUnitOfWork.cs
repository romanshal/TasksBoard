using Authentication.Domain.Interfaces.Repositories;
using Common.Blocks.Interfaces.UnitOfWorks;

namespace Authentication.Domain.Interfaces.UnitOfWorks
{
    public interface IUnitOfWork : IUnitOfWorkBase
    {
        IApplicationUserImageRepository GetApplicationUserImageRepository();
    }
}
