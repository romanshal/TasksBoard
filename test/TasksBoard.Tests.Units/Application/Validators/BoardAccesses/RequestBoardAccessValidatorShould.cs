using FluentAssertions;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Application.Validators.BoardAccesses;

namespace TasksBoard.Tests.Units.Application.Validators.BoardAccesses
{
    public class RequestBoardAccessValidatorShould
    {
        private readonly RequestBoardAccessValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new RequestBoardAccessCommand
            {
                BoardId = Guid.Parse("ee1ea84d-4849-4a0c-affa-286a0d145db3"),
                AccountId = Guid.Parse("dbe15649-33f7-4ef1-9709-a233731b94ba"),
                AccountName = "Test Account",
                AccountEmail = "testaccount@gmail.com"
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(RequestBoardAccessCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new RequestBoardAccessCommand 
            {
                BoardId = Guid.Parse("ee1ea84d-4849-4a0c-affa-286a0d145db3"),
                AccountId = Guid.Parse("dbe15649-33f7-4ef1-9709-a233731b94ba"),
                AccountName = "Test Account",
                AccountEmail = "testaccount@gmail.com"
            };

            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountName = string.Empty } };
            yield return new object[] { validCommand with { AccountName = "" } };
            yield return new object[] { validCommand with { AccountEmail = string.Empty } };
            yield return new object[] { validCommand with { AccountEmail = "" } };
            yield return new object[] { validCommand with { AccountEmail = "asdafsfas" } };
            yield return new object[] { validCommand with { AccountEmail = "asdafsfas@" } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, AccountId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, AccountId = Guid.Empty, AccountName = "" } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, AccountId = Guid.Empty, AccountName = "", AccountEmail = "" } };
        }
    }
}
