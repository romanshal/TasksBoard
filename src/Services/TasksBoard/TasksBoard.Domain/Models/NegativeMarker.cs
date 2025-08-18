namespace TasksBoard.Domain.Models
{
    public sealed record NegativeMarker
    {
        public static readonly NegativeMarker Instance = new();
        private NegativeMarker() { }
    }
}
