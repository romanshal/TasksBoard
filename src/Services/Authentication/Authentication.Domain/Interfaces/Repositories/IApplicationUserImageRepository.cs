using Authentication.Domain.Entities;
using Common.Blocks.Interfaces.Repositories;

namespace Authentication.Domain.Interfaces.Repositories
{
    public interface IApplicationUserImageRepository : IRepository<ApplicationUserImage>
    {
        Task<ApplicationUserImage?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
