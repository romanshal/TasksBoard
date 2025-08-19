namespace Common.gRPC.Models
{
    public sealed record NegativeMarker
    {
        public static readonly NegativeMarker Instance = new();
        private NegativeMarker() { }
    }
}
