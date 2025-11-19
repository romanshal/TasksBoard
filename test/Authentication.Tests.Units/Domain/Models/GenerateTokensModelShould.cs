using Authentication.Domain.Entities;
using Authentication.Domain.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authentication.Tests.Units.Domain.Models
{
    public class GenerateTokensModelShould
    {
        [Theory]
        [MemberData(nameof(GetInvalidCredentials))]
        public async Task RetunrArgumentNullExceptionException_WhenInvalidCredentials((ApplicationUser user, string userAgent, string userIp) credentials)
        {
            var userIp = credentials.userIp;
            var userAgent = credentials.userAgent;
            var user = credentials.user;

            Func<GenerateTokensModel> act = () => new GenerateTokensModel(user, userAgent, userIp);
            act.Should()
                .Throw<Exception>()
                .Where(ex => ex is ArgumentNullException || ex is ArgumentException);
        }

        public static IEnumerable<object[]> GetInvalidCredentials()
        {
            var validCredentials = (new ApplicationUser(), "TestAgent", "0.0.0.1");

            yield return new object[] { ((ApplicationUser)null, validCredentials.Item2, validCredentials.Item3) };
            yield return new object[] { (validCredentials.Item1, string.Empty, validCredentials.Item3) };
            yield return new object[] { (validCredentials.Item1, validCredentials.Item2, string.Empty) };
        }
    }
}
