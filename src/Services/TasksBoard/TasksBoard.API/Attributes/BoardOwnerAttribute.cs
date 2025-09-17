using Common.Blocks.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class BoardOwnerAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            if (user == null || !user.Identity!.IsAuthenticated)
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

            var board = await unitOfWork.GetBoardRepository().GetAsync(BoardId.Of((Guid)entityIdObj!));
            if (board is null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            if (board.OwnerId != AccountId.Of(userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
