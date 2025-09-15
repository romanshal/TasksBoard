namespace Common.Cache.Interfaces.Factories
{
    public interface ICacheKeyFactory
    {
        string Key<T>(Guid id);
    }
}