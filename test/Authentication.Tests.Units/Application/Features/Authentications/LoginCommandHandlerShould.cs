using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Application.Handlers;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class LoginCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signinManager;
        private readonly ISetup<SignInManager<ApplicationUser>, Task<SignInResult>> _signinManagerSetup;
        private readonly ISetup<UserManager<ApplicationUser>, Task<ApplicationUser>> _userManagerFindSetup;
        private readonly ISetup<UserManager<ApplicationUser>, Task<bool>> _userManagerIsLockdSetup;
        private readonly Mock<ISignInHandler> _signInHandler;
        private readonly Mock<ILogger<LoginCommandHandler>> _logger;
        private readonly LoginCommandHandler _sut;
        private readonly ISetup<ISignInHandler, Task<Result<AuthenticationDto>>> _signInHandlerSetup;

        public LoginCommandHandlerShould()
        {
            _userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _userManagerFindSetup = _userManager
                .Setup(s => s.FindByEmailAsync(It.IsAny<string>()));
            _userManagerIsLockdSetup = _userManager
                .Setup(s => s.IsLockedOutAsync(It.IsAny<ApplicationUser>()));

            _signinManager = new Mock<SignInManager<ApplicationUser>>(
                _userManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<ApplicationUser>>());
            _signinManagerSetup = _signinManager
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()));

            _signInHandler = new Mock<ISignInHandler>();
            _signInHandlerSetup = _signInHandler.Setup(s => s.HandleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            _logger = new Mock<ILogger<LoginCommandHandler>>();

            _sut = new LoginCommandHandler(_userManager.Object, _signinManager.Object, _signInHandler.Object, _logger.Object);
        }

        [Fact]
        public async Task ReturnValidAuthenticationDto_WhenUserExistAndValid()
        {
            var userId = Guid.Parse("193c2bae-715f-45be-826d-92d299710fbf");
            var command = new LoginCommand
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UsernameOrEmail = "test@gmail.com",
                Password = "Test1test",
                RememberMe = true
            };

            _userManagerFindSetup.ReturnsAsync(new ApplicationUser { EmailConfirmed = true, TwoFactorEnabled = false });
            _userManagerIsLockdSetup.ReturnsAsync(false);

            _signinManagerSetup.ReturnsAsync(SignInResult.Success);

            var authenticationDto = new AuthenticationDto
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                AccessTokenExpiredAt = DateTime.UtcNow,
                RefreshTokenExpiredAt = DateTime.UtcNow,
                UserId = userId,
                DeviceId = Guid.NewGuid().ToString(),
            };

            _signInHandlerSetup.ReturnsAsync(authenticationDto);

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNull().And.Be(authenticationDto);
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenUserDoesntExist()
        {
            var command = new LoginCommand
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UsernameOrEmail = "test@gmail.com",
                Password = "Test1test",
                RememberMe = true
            };

            _userManagerFindSetup.ReturnsAsync((ApplicationUser)null);

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Value.Should().BeNull();
            actual.Error.Should().NotBeNull();
            actual.Error.Should().Be(AuthenticationErrors.UserNotFound(command.UsernameOrEmail));
        }

        [Fact]
        public async Task ReturnEmailNotConfirmedResult_WhenEmailNotConfirmed()
        {
            var command = new LoginCommand
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UsernameOrEmail = "test@gmail.com",
                Password = "Test1test",
                RememberMe = true
            };

            _userManagerFindSetup.ReturnsAsync(new ApplicationUser { EmailConfirmed = false, TwoFactorEnabled = false });

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Value.Should().BeNull();
            actual.Error.Should().NotBeNull();
            actual.Error.Should().Be(AuthenticationErrors.EmailNotConfirmed);
        }

        [Fact]
        public async Task ReturnLockedResult_WhenUserIsLocked() 
        {
            var command = new LoginCommand
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UsernameOrEmail = "test@gmail.com",
                Password = "Test1test",
                RememberMe = true
            };

            _userManagerFindSetup.ReturnsAsync(new ApplicationUser { EmailConfirmed = true, TwoFactorEnabled = false });
            _userManagerIsLockdSetup.ReturnsAsync(true);

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Value.Should().BeNull();
            actual.Error.Should().NotBeNull();
            actual.Error.Should().Be(AuthenticationErrors.Locked);
        }

        [Fact]
        public async Task ReturnTwoFactorResult_WhneTwoFactorEnabled()
        {
            var userId = Guid.Parse("193c2bae-715f-45be-826d-92d299710fbf");
            var command = new LoginCommand
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UsernameOrEmail = "test@gmail.com",
                Password = "Test1test",
                RememberMe = true
            };

            _userManagerFindSetup.ReturnsAsync(new ApplicationUser { Id = userId, EmailConfirmed = true, TwoFactorEnabled = true });
            _userManagerIsLockdSetup.ReturnsAsync(false);

            var authenticationDto = new AuthenticationDto
            {
                AccessToken = null,
                RefreshToken = null,
                AccessTokenExpiredAt = null,
                RefreshTokenExpiredAt = null,
                UserId = userId,
                DeviceId = null,
                TwoFactorEnabled = true
            };

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNull().And.Be(authenticationDto);
        }

        [Fact]
        public async Task ReturnNotAllowedReslt_WhenSigninNotAllowed()
        {
            var userId = Guid.Parse("193c2bae-715f-45be-826d-92d299710fbf");
            var command = new LoginCommand
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UsernameOrEmail = "test@gmail.com",
                Password = "Test1test",
                RememberMe = true
            };

            _userManagerFindSetup.ReturnsAsync(new ApplicationUser { Id = userId, EmailConfirmed = true, TwoFactorEnabled = false });
            _userManagerIsLockdSetup.ReturnsAsync(false);

            _signinManagerSetup.ReturnsAsync(SignInResult.NotAllowed);

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Value.Should().BeNull();
            actual.Error.Should().NotBeNull();
            actual.Error.Should().Be(AuthenticationErrors.NotAllowed);
        }

        [Fact]
        public async Task ReturnInvalidReslt_WhenPasswordInvalid()
        {
            var userId = Guid.Parse("193c2bae-715f-45be-826d-92d299710fbf");
            var command = new LoginCommand
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UsernameOrEmail = "test@gmail.com",
                Password = "Test1test",
                RememberMe = true
            };

            _userManagerFindSetup.ReturnsAsync(new ApplicationUser { Id = userId, EmailConfirmed = true, TwoFactorEnabled = false });
            _userManagerIsLockdSetup.ReturnsAsync(false);

            _signinManagerSetup.ReturnsAsync(SignInResult.Failed);

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Value.Should().BeNull();
            actual.Error.Should().NotBeNull();
            actual.Error.Should().Be(AuthenticationErrors.Invalid);
        }
    }
}
