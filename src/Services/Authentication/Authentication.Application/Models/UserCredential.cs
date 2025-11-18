namespace Authentication.Application.Models
{
    public abstract record UserCredential
    {
        public required string UserIp { get; set; }
        public required string UserAgent { get; set; }
    }
}
