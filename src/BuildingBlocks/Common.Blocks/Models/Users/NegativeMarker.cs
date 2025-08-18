namespace Common.Blocks.Models.Users
{
    public sealed record NegativeMarker
    {
        public static readonly NegativeMarker Instance = new();
        private NegativeMarker() { }
    }
}
