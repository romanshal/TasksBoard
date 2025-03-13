using Authentication.Application.Dtos;
using Authentication.Application.Models;

namespace Authentication.Application.Interfaces.Providers
{
    public interface ITokenProvider
    {
        AuthenticationDto Create(CreateTokenModel model);
        AuthenticationDto Refresh(RefreshTokenModel request);
    }
}
