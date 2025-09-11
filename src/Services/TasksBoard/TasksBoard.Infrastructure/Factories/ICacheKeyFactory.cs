using Common.Blocks.Entities;

namespace TasksBoard.Infrastructure.Factories
{
    internal interface ICacheKeyFactory<T> where T : BaseEntity
    {
        string Key(Guid id);
    }
}