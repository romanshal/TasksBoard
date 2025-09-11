using Common.Blocks.Entities;

namespace TasksBoard.Infrastructure.Factories
{
    internal class CacheKeyFactory<T> : ICacheKeyFactory<T> where T : BaseEntity
    {
        public string Key(Guid id) => $"{nameof(T)}_{id}";
    }
}
