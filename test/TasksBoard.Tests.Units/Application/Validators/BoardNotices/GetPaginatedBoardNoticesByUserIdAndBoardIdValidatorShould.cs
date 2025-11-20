using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserIdAndBoardId;
using TasksBoard.Application.Validators.BoardNotices;

namespace TasksBoard.Tests.Units.Application.Validators.BoardNotices
{
    public class GetPaginatedBoardNoticesByUserIdAndBoardIdValidatorShould
    {
        private readonly GetPaginatedBoardNoticesByUserIdAndBoardIdValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetPaginatedBoardNoticesByUserIdAndBoardIdQuery
            {
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
                UserId = Guid.Parse("29a70119-47b8-42e4-895e-27765331f760"),
                PageIndex = 1,
                PageSize = 10
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetPaginatedBoardNoticesByUserIdAndBoardIdQuery command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetPaginatedBoardNoticesByUserIdAndBoardIdQuery
            {
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
                UserId = Guid.Parse("29a70119-47b8-42e4-895e-27765331f760"),
                PageIndex = 1,
                PageSize = 10
            };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { UserId = Guid.Empty } };
            yield return new object[] { validCommand with { PageIndex = 0 } };
            yield return new object[] { validCommand with { PageIndex = -1 } };
            yield return new object[] { validCommand with { PageSize = 0 } };
            yield return new object[] { validCommand with { PageSize = -1 } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, PageSize = 0 } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, UserId = Guid.Empty, PageSize = 0 } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, PageIndex = 0 } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, UserId = Guid.Empty, PageIndex = 0 } };
            yield return new object[] { validCommand with { PageIndex = 0, PageSize = 0 } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, UserId = Guid.Empty, PageIndex = 0, PageSize = 0 } };
        }
    }
}
