using Authentication.Application.Features.Authentications.Commands.Login;
using FluentAssertions;

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
                Username = "Test username",
                Password = "Test password",
                UserIp = "",
                UserAgent = ""
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
                Username = "Test username",
                Password = "Test password",
                UserIp = "",
                UserAgent = ""
            };
            yield return new object[] { validCommand with { Username = null } };
            yield return new object[] { validCommand with { Username = string.Empty } };
            yield return new object[] { validCommand with { Password = null } };
            yield return new object[] { validCommand with { Password = string.Empty } };
            yield return new object[] { validCommand with { Username = null, Password = null } };
        }
    }
}
