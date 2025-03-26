namespace Authentication.Application.Dtos
{
    public class AuthenticationDto : TokenDto
    {
        public Guid UserId { get; set; }
    }
}
