using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Authentication.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CurrentUserOnlyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("userId", out var entityIdObj))
            {
                context.Result = new UnprocessableEntityObjectResult("Invalid or missing entity ID.");
                return;
            }

            var user = context.HttpContext.User;

            var claims = user.Claims.ToList();

            var userId = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (userId != entityIdObj!.ToString())
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
