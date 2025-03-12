using Common.Blocks.Exceptions;
using Common.Blocks.Models;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

            return httpContext.Response.WriteAsync(responseDate.ToString());
        }

        private static HttpStatusCode GetErrorCode(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException or ValidationException => HttpStatusCode.BadRequest,
                NotFoundException => HttpStatusCode.NotFound,
                NotImplementedException => HttpStatusCode.NotImplemented,
                _ => HttpStatusCode.InternalServerError,
            };
        }

        private static Response GetResponseDate(Exception ex)
        {
            return new Response(ex.Message, true);
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
