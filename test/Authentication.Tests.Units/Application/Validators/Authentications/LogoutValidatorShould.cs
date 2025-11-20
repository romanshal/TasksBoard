using Authentication.Application.Features.Authentications.Commands.Logout;
using FluentAssertions;
using System;
using System.Collections.Generic;

namespace Authentication.Tests.Units.Application.Validators.Authentications
{
    public class LogoutValidatorShould
    {
        private readonly LogoutValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new LogoutCommand
            {
                UserId = Guid.Parse("fe6e93de-5599-4f32-a143-4a4da06e6cd3")
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(LogoutCommand command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new LogoutCommand { UserId = Guid.NewGuid() };
            yield return new object[] { validCommand with { UserId = Guid.Empty } };
        }
    }
}
