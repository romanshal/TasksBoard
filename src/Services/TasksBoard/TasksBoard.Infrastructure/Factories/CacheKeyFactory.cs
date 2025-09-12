using Common.Blocks.Entities;
using Common.Blocks.ValueObjects;

namespace TasksBoard.Infrastructure.Factories
{
    internal class CacheKeyFactory<T, TId> : ICacheKeyFactory<T, TId> where T : BaseEntity<TId> where TId : GuidValueObject<TId>
    {
        public string Key(TId id) => $"{nameof(T)}_{id.Value}";
    }
}
