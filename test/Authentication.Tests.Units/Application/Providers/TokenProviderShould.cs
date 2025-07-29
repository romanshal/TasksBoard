using Authentication.Application.Models;
using Authentication.Application.Providers;
using Authentication.Domain.Entities;
using Common.Blocks.Configurations;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace Authentication.Tests.Units.Application.Providers
{
    public class TokenProviderShould
    {
        private readonly TokenProvider sut;
        private readonly Mock<IOptions<JwtCofiguration>> options;

        public TokenProviderShould()
        {
            options = new Mock<IOptions<JwtCofiguration>>();
            options
                .Setup(s => s.Value)
                .Returns(new JwtCofiguration
                {
                    Secret = "tsdrsiggoekhmvzipakvatpczmyzhzopwfnpvqrjcvtmfhymqpsjfipinpjlonbrlskgnlxgtgqqyvtoksxpazawryuwshevaniudifaehyuttbsdedbnivmuujiuzzn",
                    Issuer = "issuer",
                    Audience = "audience",
                    ExpirationInMinutes = 1
                })
                ;
            sut = new TokenProvider(options.Object);
        }

        [Fact]
        public void ReturnCreatedTokenModel_WhenTokenCreated()
        {
            var userId = Guid.Parse("fc42da85-b8c9-4eb4-95ab-b63225bc79a2");
            var userEmail = "test@gmail.com";
            var model = new CreateTokenModel
            {
                UserId = userId,
                UserEmail = userEmail
            };

            var actual = sut.Create(model);

            actual.Should().NotBeNull();
            actual.AccessToken.Should().NotBeNullOrWhiteSpace();
            actual.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ReturnRefreshedTokenModel_WhenRefreshedTokenCreated()
        {
            var userId = Guid.Parse("fc42da85-b8c9-4eb4-95ab-b63225bc79a2");
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "Test",
                Email = "test@gmail.com"
            };
            var refreshToken = "e02c09a9-98c5-40cd-9cf8-00b2c7762ef6";
            var storedRefreshToken = "e02c09a9-98c5-40cd-9cf8-00b2c7762ef6";
            var model = new RefreshTokenModel(user, [], refreshToken, storedRefreshToken);

            var actual = sut.Refresh(model);

            actual.Should().NotBeNull();
            actual.AccessToken.Should().NotBeNullOrWhiteSpace();
            actual.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ThrowSecurityTokenException_WhenRefreshTokenInvalid()
        {
            var userId = Guid.Parse("fc42da85-b8c9-4eb4-95ab-b63225bc79a2");
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "Test",
                Email = "test@gmail.com"
            };
            var refreshToken = "asfasdf";
            var storedRefreshToken = "e02c09a9-98c5-40cd-9cf8-00b2c7762ef6";
            var model = new RefreshTokenModel(user, [], refreshToken, storedRefreshToken);

            sut
                .Invoking(s => s.Refresh(model))
                .Should()
                .Throw<SecurityTokenException>()
                .WithMessage("Invalid token.");
        }
    }
}
