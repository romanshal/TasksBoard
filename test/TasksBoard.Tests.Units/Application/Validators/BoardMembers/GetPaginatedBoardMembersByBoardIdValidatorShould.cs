﻿using FluentAssertions;
using TasksBoard.Application.Features.BoardMembers.Queries.GetPaginatedBoardMembersByBoardId;
using TasksBoard.Application.Validators.BoardMembers;

namespace TasksBoard.Tests.Units.Application.Validators.BoardMembers
{
    public class GetPaginatedBoardMembersByBoardIdValidatorShould
    {
        private readonly GetPaginatedBoardMembersByBoardIdValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetPaginatedBoardMembersByBoardIdQuery
            {
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
                PageIndex = 1,
                PageSize = 10
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetPaginatedBoardMembersByBoardIdQuery command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetPaginatedBoardMembersByBoardIdQuery
            {
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
                PageIndex = 1,
                PageSize = 10
            };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { PageIndex = 0 } };
            yield return new object[] { validCommand with { PageIndex = -1 } };
            yield return new object[] { validCommand with { PageSize = 0 } };
            yield return new object[] { validCommand with { PageSize = -1 } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, PageIndex = 0 } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, PageSize = 0 } };
            yield return new object[] { validCommand with { PageIndex = 0, PageSize = 0 } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, PageIndex = 0, PageSize = 0 } };
        }
    }
}
