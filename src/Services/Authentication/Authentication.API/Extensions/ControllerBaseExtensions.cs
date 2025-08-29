using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static void SetRefreshTokenCookies(this ControllerBase controller, string token)
        {
            controller.Response.Cookies.Append("refresh_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/authentication/refresh",
                MaxAge = TimeSpan.FromDays(7)
            });
        }
    }
}
