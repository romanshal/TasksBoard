using Authentication.Application.Features.Authentications.Commands.Login;
using FluentAssertions;
using System.Collections.Generic;

namespace Authentication.Tests.Units.Application.Validators.Authentications
{
    public class LoginValidatorShould
    {
        private readonly LoginValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new LoginCommand
            {
                UsernameOrEmail = "test@gmail.com",
                Password = "Test password",
                UserIp = "",
                UserAgent = "",
                RememberMe = true,
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(LoginCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new LoginCommand
            {
                UsernameOrEmail = "test@gmail.com",
                Password = "Test password",
                UserIp = "",
                UserAgent = "",
                RememberMe = true
            };
            yield return new object[] { validCommand with { UsernameOrEmail = null } };
            yield return new object[] { validCommand with { UsernameOrEmail = string.Empty } };
            yield return new object[] { validCommand with { Password = null } };
            yield return new object[] { validCommand with { Password = string.Empty } };
            yield return new object[] { validCommand with { UsernameOrEmail = null, Password = null } };
        }
    }
}
