namespace Authentication.Application.Models
{
    public class SessionModel : TokenModel
    {
        public required Guid SessionId { get; set; } 
    }
}
