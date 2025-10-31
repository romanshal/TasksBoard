using Authentication.Domain.Constants.TwoFactor;

namespace Authentication.Domain.Interfaces.Secutiry
{
    public interface ITwoFactorCodeSender
    {
        Task<bool> SendAsync(string provider, string address, string token);
    }
}
