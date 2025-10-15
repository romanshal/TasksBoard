namespace Common.Hubs.Interfaces
{
    public interface IConnectionService
    {
        /// <summary>
        /// Authenticate user and save its connection
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        Task Add(Guid userId, string connectionId);

        /// <summary>
        /// Authenticate user and remove its connection
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        Task Remove(Guid userId, string connectionId);

        /// <summary>
        /// Get all connected users
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<Guid, IEnumerable<string>> GetConnectedUsers();
    }
}
