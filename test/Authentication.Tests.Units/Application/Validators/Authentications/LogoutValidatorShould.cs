using Authentication.Application.Features.Authentications.Commands.Logout;
using FluentAssertions;

namespace Authentication.Tests.Units.Application.Validators.Authentications
{
    public class LogoutValidatorShould
    {
        private readonly LogoutValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new LogoutCommand
            {
                UserId = Guid.Parse("fe6e93de-5599-4f32-a143-4a4da06e6cd3")
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(LogoutCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new LogoutCommand { UserId = Guid.NewGuid() };
            yield return new object[] { validCommand with { UserId = Guid.Empty } };
        }
    }
}
