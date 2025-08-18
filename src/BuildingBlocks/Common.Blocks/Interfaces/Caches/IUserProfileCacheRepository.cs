using Common.Blocks.Models.Users;

namespace Common.Blocks.Interfaces.Caches
{
    public interface IUserProfileCacheRepository
    {
        Task SetManyAsync(IEnumerable<UserProfile> users);
        Task SetNegativeAsync(IEnumerable<Guid> missings);
        Task<IDictionary<Guid, UserProfile?>> GetManyAsync(IEnumerable<Guid> userIds);
    }
}
