﻿using FluentAssertions;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardPermissionsCommand;
using TasksBoard.Application.Validators.ManageBoardMembers;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoardMembers
{
    public class AddBoardMemberPermissionsValidatorShould
    {
        private readonly AddBoardMemberPermissionsValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new AddBoardMemberPermissionsCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                MemberId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                Permissions = [Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")]
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(AddBoardMemberPermissionsCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new AddBoardMemberPermissionsCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                MemberId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                Permissions = [Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")]
            };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty } };
            yield return new object[] { validCommand with { MemberId = Guid.Empty } };
            yield return new object[] { validCommand with { Permissions = [] } };
        }
    }
}
