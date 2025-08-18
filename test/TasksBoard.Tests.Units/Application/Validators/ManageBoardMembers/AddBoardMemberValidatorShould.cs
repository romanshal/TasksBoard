using FluentAssertions;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Application.Validators.ManageBoardMembers;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoardMembers
{
    public class AddBoardMemberValidatorShould
    {
        private readonly AddBoardMemberValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new AddBoardMemberCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(AddBoardMemberCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new AddBoardMemberCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AccountId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
            };

            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, AccountId = Guid.Empty } };
        }
    }
}
