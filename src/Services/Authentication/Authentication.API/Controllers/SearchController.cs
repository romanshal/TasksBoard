using Authentication.Application.Dtos;
using Authentication.Application.Features.Search.Queries.SearchUsers;
using Common.Blocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/search")]
    public class SearchController(
        IMediator mediator,
        ILogger<SearchController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<SearchController> _logger = logger;

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchAsync([FromQuery] string search)
        {
            var result = await _mediator.Send(new SearchUsersQuery { Query = search });

            var response = new ResultResponse<IEnumerable<UserInfoDto>>(result);

            return Ok(response);
        }
    }
}
