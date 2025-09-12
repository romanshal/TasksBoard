using Authentication.Domain.Entities;
using Authentication.Domain.ValueObjects;
using Common.Blocks.Interfaces.Repositories;

namespace Authentication.Domain.Interfaces.Repositories
{
    public interface IApplicationUserImageRepository : IRepository<ApplicationUserImage, ImageId>
    {
        Task<ApplicationUserImage?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
