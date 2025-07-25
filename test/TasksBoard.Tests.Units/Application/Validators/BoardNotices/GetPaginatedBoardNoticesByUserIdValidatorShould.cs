using FluentAssertions;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId;
using TasksBoard.Application.Validators.BoardNotices;

namespace TasksBoard.Tests.Units.Application.Validators.BoardNotices
{
    public class GetPaginatedBoardNoticesByUserIdValidatorShould
    {
        private readonly GetPaginatedBoardNoticesByUserIdValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetPaginatedBoardNoticesByUserIdQuery
            {
                UserId = Guid.Parse("29a70119-47b8-42e4-895e-27765331f760"),
                PageIndex = 1,
                PageSize = 10
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetPaginatedBoardNoticesByUserIdQuery command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetPaginatedBoardNoticesByUserIdQuery
            {
                UserId = Guid.Parse("29a70119-47b8-42e4-895e-27765331f760"),
                PageIndex = 1,
                PageSize = 10
            };
            yield return new object[] { validCommand with { UserId = Guid.Empty } };
            yield return new object[] { validCommand with { PageIndex = 0 } };
            yield return new object[] { validCommand with { PageIndex = -1 } };
            yield return new object[] { validCommand with { PageSize = 0 } };
            yield return new object[] { validCommand with { PageSize = -1 } };
            yield return new object[] { validCommand with { UserId = Guid.Empty, PageSize = 0 } };
            yield return new object[] { validCommand with { UserId = Guid.Empty, PageIndex = 0 } };
            yield return new object[] { validCommand with { PageIndex = 0, PageSize = 0 } };
            yield return new object[] { validCommand with { UserId = Guid.Empty, PageIndex = 0, PageSize = 0 } };
        }
    }
}
