using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Application.Features.Authentications.Commands.Register;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class RegisterCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> userManager;
        private readonly Mock<SignInManager<ApplicationUser>> signinManager;
        private readonly Mock<ITokenService> tokenService;
        private readonly Mock<ILogger<RegisterCommandHandler>> logger;
        private readonly RegisterCommandHandler sut;

        public RegisterCommandHandlerShould()
        {
            userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            signinManager = new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<ApplicationUser>>());

            tokenService = new Mock<ITokenService>();

            logger = new Mock<ILogger<RegisterCommandHandler>>();

            sut = new RegisterCommandHandler(userManager.Object, signinManager.Object, tokenService.Object, logger.Object);
        }

        [Fact]
        public async Task ReturnAuthenticatedModel_WhenUserRegistered()
        {
            var userName = "Test";
            var email = "test@gmail.com";
            var command = new RegisterCommand
            {
                Username = userName,
                Email = email,
                Password = string.Empty
            };

            var token = new TokenDto
            {
                AccessToken = "ddf564f1-6cb9-4337-9a4c-5956e7189f53",
                RefreshToken = "ddf564f1-6cb9-4337-9a4c-5956e7189f53"
            };

            userManager
                .Setup(s => s.FindByNameAsync(userName))
                .ReturnsAsync(value: null);

            userManager
                .Setup(s => s.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(s => s.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            signinManager
                .Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            tokenService
                .Setup(s => s.GenerateTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(token);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.Should().NotBeNull();
            actual.AccessToken.Should().NotBeNullOrWhiteSpace();
            actual.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ThrowAlreadyExistException_WhenUserAlreadyExist()
        {
            var userName = "Test";
            var email = "test@gmail.com";
            var command = new RegisterCommand
            {
                Username = userName,
                Email = email,
                Password = string.Empty
            };

            userManager
                .Setup(s => s.FindByNameAsync(userName))
                .ReturnsAsync(new ApplicationUser { UserName = userName });

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<AlreadyExistException>()
                .WithMessage($"User with name {userName} is already exist.");
        }
    }
}
