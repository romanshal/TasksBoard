using Authentication.Application.Dtos;
using Authentication.Application.Models;

namespace Authentication.Application.Interfaces.Providers
{
    public interface ITokenProvider
    {
        TokenDto Create(CreateTokenModel model);
        TokenDto Refresh(RefreshTokenModel request);
    }
}
