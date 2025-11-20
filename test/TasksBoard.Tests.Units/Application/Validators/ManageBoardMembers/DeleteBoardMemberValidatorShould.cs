using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember;
using TasksBoard.Application.Validators.ManageBoardMembers;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoardMembers
{
    public class DeleteBoardMemberValidatorShould
    {
        private readonly DeleteBoardMemberValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new DeleteBoardMemberCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                MemberId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                RemoveByUserId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(DeleteBoardMemberCommand command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new DeleteBoardMemberCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                MemberId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                RemoveByUserId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")
            };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { MemberId = Guid.Empty } };
            yield return new object[] { validCommand with { RemoveByUserId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, MemberId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, MemberId = Guid.Empty, RemoveByUserId = Guid.Empty } };
        }
    }
}
