namespace Common.Blocks.Entities
{
    public interface IEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }

    public interface IEntity<T> : IEntity
    {
        public T Id { get; set; }
    }
}
