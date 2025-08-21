namespace Authentication.Application.Dtos
{
    public class AuthenticationDto : TokenPairDto
    {
        public Guid UserId { get; set; }
    }
}
