using FluentAssertions;
using TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById;
using TasksBoard.Application.Validators.BoardNotices;

namespace TasksBoard.Tests.Units.Application.Validators.BoardNotices
{
    public class GetBoardNoticeByIdValidatorShould
    {
        private readonly GetBoardNoticeByIdValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetBoardNoticeByIdQuery
            {
                Id = Guid.Parse("f2e437c3-a4d3-409e-aa12-592ca1445903")
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetBoardNoticeByIdQuery command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetBoardNoticeByIdQuery
            {
                Id = Guid.Parse("cd5e2136-cfb7-4c36-bb97-97be589a9ce1"),
            };
            yield return new object[] { validCommand with { Id = Guid.Empty } };
        }
    }
}
