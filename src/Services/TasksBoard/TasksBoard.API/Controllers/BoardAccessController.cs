using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TasksBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/boardaccess")]
    public class BoardAccessController(
        ILogger<BoardAccessController> logger,
        IMediator mediator) : ControllerBase
    {
        private readonly ILogger<BoardAccessController> _logger = logger;
        private readonly IMediator _mediator = mediator;

    }
}
