using Authentication.API.Attributes;
using Authentication.Application.Features.Manage.Queries.GetUserInfo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/manage")]
    public class ManageController(
        IMediator mediator,
        ILogger<ManageController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ManageController> _logger = logger;

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserInfoAsync([FromRoute] Guid userId)
        {
            var result = await _mediator.Send(new GetUserInfoQuery { UserId = userId });
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
