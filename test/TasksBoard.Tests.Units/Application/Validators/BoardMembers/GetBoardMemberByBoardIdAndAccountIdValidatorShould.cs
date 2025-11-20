using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMemberByBoardIdAndAccountId;
using TasksBoard.Application.Validators.BoardMembers;

namespace TasksBoard.Tests.Units.Application.Validators.BoardMembers
{
    public class GetBoardMemberByBoardIdAndAccountIdValidatorShould
    {
        private readonly GetBoardMemberByBoardIdAndAccountIdValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetBoardMemberByBoardIdAndAccountIdQuery
            {
                AccountId = Guid.Parse("cd5e2136-cfb7-4c36-bb97-97be589a9ce1"),
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5")
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetBoardMemberByBoardIdAndAccountIdQuery command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetBoardMemberByBoardIdAndAccountIdQuery
            {
                AccountId = Guid.Parse("cd5e2136-cfb7-4c36-bb97-97be589a9ce1"),
                BoardId = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
            };
            yield return new object[] { validCommand with { AccountId = Guid.Empty } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { AccountId = Guid.Empty, BoardId = Guid.Empty } };
        }
    }
}
