using Common.Blocks.Entities;
using Common.Blocks.ValueObjects;

namespace TasksBoard.Infrastructure.Factories
{
    internal interface ICacheKeyFactory<T, TId> where T : BaseEntity<TId> where TId : GuidValueObject<TId>
    {
        string Key(TId id);
    }
}