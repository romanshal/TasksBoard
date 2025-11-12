using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest;
using TasksBoard.Application.Validators.BoardInvites;

namespace TasksBoard.Tests.Units.Application.Validators.BoardInvites
{
    public class ResolveInviteRequestValidatorShould
    {
        private readonly ResolveInviteRequestValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new ResolveInviteRequestCommand
            {
                RequestId = Guid.Parse("cd5e2136-cfb7-4c36-bb97-97be589a9ce1"),
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
                Decision = true
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(ResolveInviteRequestCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new ResolveInviteRequestCommand
            {
                RequestId = Guid.Parse("cd5e2136-cfb7-4c36-bb97-97be589a9ce1"),
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
                Decision = true
            };
            yield return new object[] { validCommand with { RequestId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { RequestId = Guid.Empty, BoardId = Guid.Empty } };
        }
    }
}
