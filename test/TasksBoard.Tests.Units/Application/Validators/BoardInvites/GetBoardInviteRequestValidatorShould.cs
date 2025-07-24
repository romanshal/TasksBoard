using FluentAssertions;
using TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId;
using TasksBoard.Application.Validators.BoardInvites;

namespace TasksBoard.Tests.Units.Application.Validators.BoardInvites
{
    public class GetBoardInviteRequestValidatorShould
    {
        private readonly GetBoardInviteRequestValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetBoardInviteRequestByToAccountIdQuery
            {
                AccountId = Guid.Parse("cd5e2136-cfb7-4c36-bb97-97be589a9ce1")
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetBoardInviteRequestByToAccountIdQuery command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetBoardInviteRequestByToAccountIdQuery { AccountId = Guid.NewGuid() };
            yield return new object[] { validCommand with { AccountId = Guid.Empty } };
        }
    }
}
