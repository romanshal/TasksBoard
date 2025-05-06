using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HasBoardPermissionAttribute(string permission) : Attribute, IAsyncActionFilter
    {
        private readonly string _permission = permission;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("boardId", out var entityIdObj))
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

            using var scope = context.HttpContext.RequestServices.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var permissions = await unitOfWork.GetRepository<BoardPermission>().GetAllAsync();
            if (!permissions.Any(permission => permission.Name == _permission))
            {
                context.Result = new UnprocessableEntityObjectResult($"Permission with name '{_permission}' not found");
                return;
            }

            var boardMember = await unitOfWork.GetBoardMemberRepository().GetByBoardIdAndAccountIdAsync((Guid)entityIdObj!, Guid.Parse(userId));
            if (boardMember is null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission = boardMember.BoardMemberPermissions.Any(permission => permission.BoardPermission.Name == _permission);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
