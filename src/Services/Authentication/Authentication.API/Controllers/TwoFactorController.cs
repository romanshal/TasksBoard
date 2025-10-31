using Authentication.API.Extensions;
using Authentication.Application.Features.Authentications.Commands.GenerateAuthenticatorSetup;
using Authentication.Application.Features.Authentications.Commands.GenerateTwoFactorCode;
using Authentication.Application.Features.Authentications.Commands.VerifyTwoFactor;
using AutoMapper;
using Common.Blocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Route("api/twofactor")]
    public class TwoFactorController(
        IMediator mediator,
        IMapper mapper) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IMapper _mapper = mapper;

        [HttpGet("authenticator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateAuthenticatorSetupASync(
            [FromQuery] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GenerateAuthenticatorSetupCommand(userId, ""), cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpGet("code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateTwoFactorCodeAsync(
            [FromQuery] Guid userId,
            [FromQuery] string provider,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GenerateTwoFactorCodeCommand(userId, provider), cancellationToken);

            return this.HandleResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VirifyCodeAsync(VerifyTwoFactorCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return this.MapResponse(result, _mapper);
        }
    }
}
