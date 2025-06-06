using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Dtos;
using Notification.Application.Features.Notifications.Commands.SetNotificationsRead;
using Notification.Application.Features.Notifications.Queries.GetNewNotificationsByAccountId;
using Notification.Application.Features.Notifications.Queries.GetNewPaginatedNotificationsByAccountId;
using Notification.Application.Features.Notifications.Queries.GetPaginatedNotificationsByAccountId;

namespace Notification.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/notifications")]
    public class NotificationController(
        ILogger<NotificationController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger = logger;
        private readonly IMediator _mediator = mediator;

        [HttpGet("all/paginated/{accountId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaginatedNotificationsByAccountIdAsync([FromRoute] Guid accountId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllPaginatedNotificationsByAccountIdQuery
            {
                AccountId = accountId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            var response = new ResultResponse<PaginatedList<NotificationDto>>(result);

            return Ok(response);
        }

        [HttpGet("new/paginated/{accountId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNewPaginatedNotificationsByAccountIdAsync([FromRoute] Guid accountId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetNewPaginatedNotificationsByAccountIdQuery
            {
                AccountId = accountId,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            var response = new ResultResponse<PaginatedList<NotificationDto>>(result);

            return Ok(response);
        }

        [HttpGet("new/{accountId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNewNotificationsByAccountIdAsync([FromRoute] Guid accountId)
        {
            var result = await _mediator.Send(new GetNewNotificationsByAccountIdQuery
            {
                AccountId = accountId
            });

            var response = new ResultResponse<IEnumerable<NotificationDto>>(result);

            return Ok(response);
        }

        [HttpPost("{accountId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetNotificationsReadAsync([FromRoute] Guid accountId, [FromBody] Guid[] notificationIds)
        {
            var result = await _mediator.Send(new SetNotificationsReadCommand
            {
                AccountId = accountId,
                NotificationIds = notificationIds
            });

            var response = new Response();

            return Ok(response);
        }
    }
}
