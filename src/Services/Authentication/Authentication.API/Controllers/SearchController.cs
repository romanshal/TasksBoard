using Authentication.API.Models.Responses;
using Authentication.Application.Features.Search.Queries.SearchUsersByQuery;
using AutoMapper;
using Common.Blocks.Models.ApiResponses;
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
        IMapper mapper) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IMapper _mapper = mapper;

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchAsync([FromQuery] string search, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new SearchUsersByQueryQuery { Query = search }, cancellationToken);

            var responseModel = _mapper.Map<IEnumerable<SearchResponse>>(result);

            var response = ApiResponse.Success(responseModel);

            return Ok(response);
        }
    }
}
