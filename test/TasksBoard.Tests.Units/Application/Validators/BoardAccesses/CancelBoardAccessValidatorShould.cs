using FluentAssertions;
using TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess;
using TasksBoard.Application.Validators.BoardAccesses;

namespace TasksBoard.Tests.Units.Application.Validators.BoardAccesses
{
    public class CancelBoardAccessValidatorShould
    {
        private readonly CancelBoardAccessValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new CancelBoardAccessCommand
            {
                RequestId = Guid.Parse("cd5e2136-cfb7-4c36-bb97-97be589a9ce1")
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(CancelBoardAccessCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new CancelBoardAccessCommand { RequestId = Guid.NewGuid() };
            yield return new object[] { validCommand with { RequestId = Guid.Empty } };
        }
    }
}
