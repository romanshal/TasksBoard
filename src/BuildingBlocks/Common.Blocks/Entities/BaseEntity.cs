using Common.Blocks.ValueObjects;

namespace Common.Blocks.Entities
{
    public abstract class BaseEntity<T> : IEntity<T>
    {
        public T Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
