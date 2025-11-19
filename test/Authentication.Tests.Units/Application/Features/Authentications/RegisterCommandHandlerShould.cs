using Authentication.Application.Features.Authentications.Commands.Register;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using System.Threading;
using System.Threading.Tasks;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class RegisterCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<IMediator> _mediator;
        private readonly ISetup<IMediator, Task<Result>> _mediatorSetup;
        private readonly Mock<ILogger<RegisterCommandHandler>> _logger;
        private readonly RegisterCommandHandler _sut;

        public RegisterCommandHandlerShould()
        {
            _userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _mediator = new Mock<IMediator>();
            _mediatorSetup = _mediator.Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()));

            _logger = new Mock<ILogger<RegisterCommandHandler>>();

            _sut = new RegisterCommandHandler(_userManager.Object, _mediator.Object, _logger.Object);
        }
    }
}
