using Common.Blocks.Configurations;
using Microsoft.Extensions.Options;
using Moq;

namespace Authentication.Tests.Units.Application.Providers
{
    public class TokenProviderShould
    {
        //private readonly TokenProvider sut;
        private readonly Mock<IOptions<JwtCofiguration>> _options;

        public TokenProviderShould()
        {
            _options = new Mock<IOptions<JwtCofiguration>>();
            _options
                .Setup(s => s.Value)
                .Returns(new JwtCofiguration
                {
                    Secret = "tsdrsiggoekhmvzipakvatpczmyzhzopwfnpvqrjcvtmfhymqpsjfipinpjlonbrlskgnlxgtgqqyvtoksxpazawryuwshevaniudifaehyuttbsdedbnivmuujiuzzn",
                    Issuer = "issuer",
                    Audience = "audience",
                    AccessTokenExpirationMinutes = 1,
                    RefreshTokenExpirationDays = 1
                });

            //sut = new TokenProvider(options.Object);
        }
    }
}
