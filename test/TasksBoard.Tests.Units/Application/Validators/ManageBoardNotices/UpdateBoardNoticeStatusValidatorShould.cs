using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNoticeStatus;
using TasksBoard.Application.Validators.ManageBoardNotices;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoardNotices
{
    public class UpdateBoardNoticeStatusValidatorShould
    {
        private readonly UpdateBoardNoticeStatusValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new UpdateBoardNoticeStatusCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountName = "Test account name",
                NoticeId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(UpdateBoardNoticeStatusCommand command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new UpdateBoardNoticeStatusCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountName = "Test account name",
                NoticeId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
            };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountName = "" } };
            yield return new object[] { validCommand with { AccountName = null } };
            yield return new object[] { validCommand with { NoticeId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, AccountId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, AccountId = Guid.Empty, AccountName = null } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, AccountId = Guid.Empty, AccountName = null, NoticeId = Guid.Empty } };
        }
    }
}
