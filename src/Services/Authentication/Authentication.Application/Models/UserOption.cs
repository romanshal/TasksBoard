namespace Authentication.Application.Models
{
    public abstract record UserOption
    {
        public required string UserIp { get; set; }
        public required string UserAgent { get; set; }
        public required string DeviceId { get; set; }
    }
}
