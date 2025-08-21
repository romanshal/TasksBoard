using Authentication.Domain.Entities;

namespace Authentication.Domain.Interfaces.Repositories
{
    public interface IApplicationUserSessionRepository
    {
        Task<IEnumerable<ApplicationUserSession>> GetByUserIdAsync(
            Guid userId, 
            CancellationToken cancellationToken = default);

        Task<ApplicationUserSession?> GetByUserIdAndDeviceId(
            Guid userId,
            string deviceId,
            CancellationToken cancellationToken = default);

        void Add(ApplicationUserSession token);

        void Update(ApplicationUserSession token);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
