using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Application.Validators.Boards;

namespace TasksBoard.Tests.Units.Application.Validators.Boards
{
    public class GetBoardByIdValidatorShould
    {
        private readonly GetBoardByIdValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new GetBoardByIdQuery
            {
                Id = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(GetBoardByIdQuery command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new GetBoardByIdQuery
            {
                Id = Guid.Parse("dd2665b6-69e6-45c1-a3ed-3ab9ba78eae5"),
            };
            yield return new object[] { validCommand with { Id = Guid.Empty } };
        }
    }
}
