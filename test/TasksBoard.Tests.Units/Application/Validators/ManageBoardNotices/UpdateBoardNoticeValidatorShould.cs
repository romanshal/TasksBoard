using FluentAssertions;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice;
using TasksBoard.Application.Validators.ManageBoardNotices;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoardNotices
{
    public class UpdateBoardNoticeValidatorShould
    {
        private readonly UpdateBoardNoticeValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new UpdateBoardNoticeCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                NoticeId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                Definition = "Test definition",
                BackgroundColor = "#111111",
                Rotation = "100deg"
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(UpdateBoardNoticeCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new UpdateBoardNoticeCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                NoticeId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                Definition = "Test definition",
                BackgroundColor = "Test color",
                Rotation = "100deg"
            };
            yield return new object[] { validCommand with { AccountId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { Definition = "" } };
            yield return new object[] { validCommand with { Definition = null } };
            yield return new object[] { validCommand with { BackgroundColor = "" } };
            yield return new object[] { validCommand with { BackgroundColor = "adasd" } };
            yield return new object[] { validCommand with { BackgroundColor = null } };
            yield return new object[] { validCommand with { Rotation = "" } };
            yield return new object[] { validCommand with { Rotation = "adasd" } };
            yield return new object[] { validCommand with { Rotation = null } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty, BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty, BoardId = Guid.Empty, NoticeId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty, BoardId = Guid.Empty, NoticeId = Guid.Empty, Definition = null } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty, BoardId = Guid.Empty, NoticeId = Guid.Empty, Definition = null, BackgroundColor = null } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty, BoardId = Guid.Empty, NoticeId = Guid.Empty, Definition = null, BackgroundColor = null, Rotation = null } };
        }
    }
}
