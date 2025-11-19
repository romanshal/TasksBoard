using Authentication.Application.Features.Authentications.Commands.GenerateEmailConfirmToken;
using Authentication.Application.Features.Authentications.Commands.Register;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class RegisterCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly ISetup<UserManager<ApplicationUser>, Task<ApplicationUser>> _userManagerFindSetup;
        private readonly Mock<IMediator> _mediator;
        private readonly ISetup<IMediator, Task<Result>> _mediatorSetup;
        private readonly Mock<ILogger<RegisterCommandHandler>> _logger;
        private readonly RegisterCommandHandler _sut;
        private readonly ISetup<UserManager<ApplicationUser>, Task<IdentityResult>> _userManagerCreateSetup;
        private readonly ISetup<UserManager<ApplicationUser>, Task<IdentityResult>> _userManagerAddRoleSetup;

        public RegisterCommandHandlerShould()
        {
            _userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _userManagerFindSetup = _userManager
                .Setup(s => s.FindByNameAsync(It.IsAny<string>()));
            _userManagerCreateSetup = _userManager
                .Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));
            _userManagerAddRoleSetup = _userManager
                .Setup(s => s.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));

            _mediator = new Mock<IMediator>();
            _mediatorSetup = _mediator.Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()));

            _logger = new Mock<ILogger<RegisterCommandHandler>>();

            _sut = new RegisterCommandHandler(_userManager.Object, _mediator.Object, _logger.Object);
        }

        [Fact]
        public async Task ReturnSuccessResult_WhenUserCreated()
        {
            var userId = Guid.Parse("193c2bae-715f-45be-826d-92d299710fbf");
            var command = new RegisterCommand
            {
                Username = "Test",
                Email = "test@gmail.com",
                Password = "Test1test",
            };

            _userManagerFindSetup.ReturnsAsync((ApplicationUser)null);

            _userManagerCreateSetup.ReturnsAsync(IdentityResult.Success);

            _userManagerAddRoleSetup.ReturnsAsync(IdentityResult.Success);

            _mediator
                .Setup(s => s.Send(It.IsAny<GenerateEmailConfirmTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success);

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnAlreadyExistResult_WhenUserExist()
        {
            var userId = Guid.Parse("193c2bae-715f-45be-826d-92d299710fbf");
            var command = new RegisterCommand
            {
                Username = "Test",
                Email = "test@gmail.com",
                Password = "Test1test",
            };

            _userManagerFindSetup.ReturnsAsync(new ApplicationUser());

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().Be(AuthenticationErrors.AlreadyExist(command.Username));
        }

        [Fact]
        public async Task ReturnSignupFaultedResult_WhenCreateUserFaulted()
        {
            var command = new RegisterCommand
            {
                Username = "Test",
                Email = "test@gmail.com",
                Password = "Test1test",
            };

            _userManagerFindSetup.ReturnsAsync((ApplicationUser)null);

            _userManagerCreateSetup.ReturnsAsync(IdentityResult.Failed());

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().Be(AuthenticationErrors.SignupFaulted);
        }

        [Fact]
        public async Task ReturnSignupFaultedResult_WhenAddRoleFaulted()
        {
            var command = new RegisterCommand
            {
                Username = "Test",
                Email = "test@gmail.com",
                Password = "Test1test",
            };

            _userManagerFindSetup.ReturnsAsync((ApplicationUser)null);

            _userManagerCreateSetup.ReturnsAsync(IdentityResult.Success);

            _userManagerAddRoleSetup.ReturnsAsync(IdentityResult.Failed());

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().Be(AuthenticationErrors.SignupFaulted);
        }

        [Fact]
        public async Task ReturnSignupFaultedResult_WhenSendEmailFaulted()
        {
            var command = new RegisterCommand
            {
                Username = "Test",
                Email = "test@gmail.com",
                Password = "Test1test",
            };

            _userManagerFindSetup.ReturnsAsync((ApplicationUser)null);

            _userManagerCreateSetup.ReturnsAsync(IdentityResult.Success);

            _userManagerAddRoleSetup.ReturnsAsync(IdentityResult.Success);

            _mediator
                .Setup(s => s.Send(It.IsAny<GenerateEmailConfirmTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure(null));

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().Be(AuthenticationErrors.SignupFaulted);
        }
    }
}
