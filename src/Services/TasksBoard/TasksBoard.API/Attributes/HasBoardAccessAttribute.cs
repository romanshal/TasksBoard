using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HasBoardAccessAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var claims = user.Claims.ToList();
            var userId = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!context.ActionArguments.TryGetValue("boardId", out var entityIdObj))
            {
                context.Result = new UnprocessableEntityObjectResult("Invalid or missing entity ID.");
                return;
            }

            using var scope = context.HttpContext.RequestServices.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var hasAccess = await unitOfWork.GetBoardRepository().HasAccessAsync((Guid)entityIdObj!, Guid.Parse(userId));

            if (!hasAccess)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
