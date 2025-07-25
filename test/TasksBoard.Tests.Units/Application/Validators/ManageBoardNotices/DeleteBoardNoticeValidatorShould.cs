using FluentAssertions;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand;
using TasksBoard.Application.Validators.ManageBoardNotices;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoardNotices
{
    public class DeleteBoardNoticeValidatorShould
    {
        private readonly DeleteBoardNoticeValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new DeleteBoardNoticeCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                NoticeId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(DeleteBoardNoticeCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new DeleteBoardNoticeCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                NoticeId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")
            };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { NoticeId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, NoticeId = Guid.Empty } };
        }
    }
}
