using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMembersByBoardId;
using TasksBoard.Application.Validators.BoardMembers;

namespace TasksBoard.Tests.Units.Application.Validators.BoardMembers
{
    public class GetBoardMembersByBoardIdValidatorShould
    {
        private readonly GetBoardMembersByBoardIdValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetBoardMembersByBoardIdQuery
            {
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetBoardMembersByBoardIdQuery command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetBoardMembersByBoardIdQuery
            {
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
            };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
        }
    }
}
