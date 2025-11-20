using Authentication.Application.Features.Authentications.Commands.RefreshToken;
using FluentAssertions;
using System;
using System.Collections.Generic;

namespace Authentication.Tests.Units.Application.Validators.Authentications
{
    public class RefreshTokenValidatorShould
    {
        private readonly RefreshTokenValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new RefreshTokenCommand
            {
                UserId = Guid.Parse("fe6e93de-5599-4f32-a143-4a4da06e6cd3"),
                RefreshToken = "fe6e93de-5599-4f32-a143-4a4da06e6cd3",
                UserIp = "",
                UserAgent = "",
                DeviceId = ""
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(RefreshTokenCommand command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new RefreshTokenCommand
            {
                UserId = Guid.Parse("fe6e93de-5599-4f32-a143-4a4da06e6cd3"),
                RefreshToken = "fe6e93de-5599-4f32-a143-4a4da06e6cd3",
                UserIp = "",
                UserAgent = "",
                DeviceId = ""
            };
            yield return new object[] { validCommand with { UserId = Guid.Empty } };
            yield return new object[] { validCommand with { RefreshToken = null } };
            yield return new object[] { validCommand with { RefreshToken = string.Empty } };
            yield return new object[] { validCommand with { UserId = Guid.Empty, RefreshToken = string.Empty } };
        }
    }
}
