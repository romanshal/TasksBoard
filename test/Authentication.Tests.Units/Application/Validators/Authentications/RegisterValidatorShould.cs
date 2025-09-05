using Authentication.Application.Features.Authentications.Commands.Register;
using Authentication.Application.Validators.Authentications;
using FluentAssertions;

namespace Authentication.Tests.Units.Application.Validators.Authentications
{
    public class RegisterValidatorShould
    {
        private readonly RegisterValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new RegisterCommand
            {
                Username = "Test username",
                Email = "testemail@gmail.com",
                Password = "Test password",
                UserIp = "",
                UserAgent = ""
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(RegisterCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new RegisterCommand
            {
                Username = "Test username",
                Email = "testemail@gmail.com",
                Password = "Test password",
                UserIp = "",
                UserAgent = ""
            };
            yield return new object[] { validCommand with { Username = null } };
            yield return new object[] { validCommand with { Username = string.Empty } };
            yield return new object[] { validCommand with { Email = null } };
            yield return new object[] { validCommand with { Email = string.Empty } };
            yield return new object[] { validCommand with { Email = "dsfsdfsdf" } };
            yield return new object[] { validCommand with { Password = null } };
            yield return new object[] { validCommand with { Password = string.Empty } };
            yield return new object[] { validCommand with { Username = null, Email = string.Empty } };
            yield return new object[] { validCommand with { Username = null, Email = string.Empty, Password = null } };
        }
    }
}
