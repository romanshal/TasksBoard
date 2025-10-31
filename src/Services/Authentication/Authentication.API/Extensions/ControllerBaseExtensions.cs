using Authentication.API.Models.Responses;
using Authentication.Application.Dtos;
using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models.ApiResponses;
using Common.Blocks.Models.DomainResults;
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

        public static IActionResult MapResponse(this ControllerBase controller, Result<AuthenticationDto> result, IMapper mapper)
        {
            if (result.IsFailure)
            {
                return controller.MapErrors(result.Error);
            }

            controller.SetRefreshTokenCookies(result.Value!.RefreshToken!);

            var response = mapper.Map<AuthenticationResponse>(result.Value);

            return controller.Ok(ApiResponse.Success(response));
        }
    }
}
