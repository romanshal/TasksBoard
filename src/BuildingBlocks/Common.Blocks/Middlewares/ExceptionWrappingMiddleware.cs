using Common.Blocks.Exceptions;
using Common.Blocks.Models;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Reflection.Metadata.Ecma335;

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

        private static Response GetResponseDate(Exception ex)
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
                or SecurityTokenException => new Response(ex.Message, true),

                _ => new Response("Something went wrong...", true),
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
