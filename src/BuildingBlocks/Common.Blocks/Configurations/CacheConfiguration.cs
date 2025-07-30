namespace Common.Blocks.Configurations
{
    public record CacheConfiguration
    {
        public required RedisConfiguration Redis { get; init; }
        public required int ExpirationTimeSeconds { get; init; }
    }
}
