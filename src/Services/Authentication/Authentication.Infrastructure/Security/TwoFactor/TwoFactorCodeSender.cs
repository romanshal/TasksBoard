using Authentication.Domain.Interfaces.Secutiry;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Infrastructure.Security.TwoFactor
{
    internal class TwoFactorCodeSender : ITwoFactorCodeSender
    {
        public async Task<bool> SendAsync(string provider, string address, string token)
        {
            if (provider == TokenOptions.DefaultPhoneProvider)
            {
                return await SendSmsAsync(address, token);
            }
            else if (provider == TokenOptions.DefaultEmailProvider)
            {
                return await SendEmailAsync(address, token);
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string address, string token)
        {
            return true;
        }

        private async Task<bool> SendSmsAsync(string phone, string token)
        {
            return true;
        }
    }
}
