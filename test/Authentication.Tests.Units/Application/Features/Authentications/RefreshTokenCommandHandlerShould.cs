using Authentication.Application.Features.Authentications.Commands.RefreshToken;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class RefreshTokenCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> userManager;
        private readonly Mock<ITokenManager> tokenService;
        private readonly Mock<ILogger<RefreshTokenCommandHandler>> logger;
        private readonly RefreshTokenCommandHandler sut;

        //public RefreshTokenCommandHandlerShould()
        //{
        //    userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

        //    tokenService = new Mock<ITokenManager>();

        //    logger = new Mock<ILogger<RefreshTokenCommandHandler>>();

        //    sut = new RefreshTokenCommandHandler(userManager.Object, tokenService.Object, logger.Object);
        //}
    }
}
