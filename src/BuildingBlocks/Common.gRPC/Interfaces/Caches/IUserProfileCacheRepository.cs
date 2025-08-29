using Common.gRPC.Models;

namespace Common.gRPC.Interfaces.Caches
{
    public interface IUserProfileCacheRepository
    {
        Task SetManyAsync(HashSet<UserProfile> users);
        Task SetNegativeAsync(HashSet<Guid> missings);
        Task<IDictionary<Guid, UserProfile?>> GetManyAsync(HashSet<Guid> userIds);
    }
}
