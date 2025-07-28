using Common.Blocks.Exceptions;
using Common.Blocks.Models.ApiResponses;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Common.Blocks.Middlewares
{
    public class ExceptionWrappingMiddleware(RequestDelegate requestDelegate)
    {
        private readonly RequestDelegate _requestDelegate = requestDelegate;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await this._requestDelegate(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExeptionAsync(httpContext, ex);
            }
        }

        private Task HandleExeptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = (int)GetErrorCode(ex);
            var responseDate = GetResponseDate(ex);

            httpContext.Response.ContentType = "application/json";
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseDate);

            return httpContext.Response.WriteAsync(jsonResponse);
        }

        private static HttpStatusCode GetErrorCode(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException or ValidationException or AlreadyExistException or InvalidPasswordException => HttpStatusCode.BadRequest,
                NotFoundException => HttpStatusCode.NotFound,
                NotImplementedException => HttpStatusCode.NotImplemented,
                LockedException => HttpStatusCode.Locked,
                SigninFaultedException or UnauthorizedException or SecurityTokenException => HttpStatusCode.Unauthorized,
                ForbiddenException => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.InternalServerError,
            };
        }

        private static ApiResponse GetResponseDate(Exception ex)
        {
            return ex switch
            {
                ValidationException
                or AlreadyExistException
                or InvalidPasswordException
                or NotFoundException
                or LockedException
                or SigninFaultedException
                or UnauthorizedException
                or SecurityTokenException => ApiResponse.Error(ex.Message),

                _ => ApiResponse.Error("Something went wrong..."),
            };
        }
    }

    public static class ExeptionWrappingMiddlewareExtention
    {
        public static IApplicationBuilder UseExeptionWrappingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionWrappingMiddleware>();
        }
    }
}
