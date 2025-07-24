using FluentAssertions;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;
using TasksBoard.Application.Validators.BoardNotices;

namespace TasksBoard.Tests.Units.Application.Validators.BoardNotices
{
    public class GetPaginatedListValidatorShould
    {
        private readonly GetPaginatedListValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetPaginatedListQuery<BoardNoticeDto>
            {
                PageIndex = 1,
                PageSize = 10
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetPaginatedListQuery<BoardNoticeDto> command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetPaginatedListQuery<BoardNoticeDto>
            {
                PageIndex = 1,
                PageSize = 10
            };

            yield return new object[] { validCommand with { PageIndex = 0 } };
            yield return new object[] { validCommand with { PageIndex = -1 } };
            yield return new object[] { validCommand with { PageSize = 0 } };
            yield return new object[] { validCommand with { PageSize = -1 } };
            yield return new object[] { validCommand with { PageIndex = 0, PageSize = 0 } };
        }
    }
}
