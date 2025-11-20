using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Repositories;
using Authentication.Domain.Interfaces.UnitOfWorks;
using Authentication.Domain.ValueObjects;
using Authentication.Infrastructure.Data.Contexts;
using Common.Blocks.UnitOfWorks;

namespace Authentication.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        IServiceProvider serviceProvider) : UnitOfWorkBase<AuthenticationDbContext>(serviceProvider), IUnitOfWork
    {
        public IApplicationUserImageRepository GetApplicationUserImageRepository()
        {
            return base.GetRepository<ApplicationUserImage, ImageId, IApplicationUserImageRepository>();
        }
    }
}
