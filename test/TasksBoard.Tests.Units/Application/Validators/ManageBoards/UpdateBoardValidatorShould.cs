using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard;
using TasksBoard.Application.Validators.ManageBoards;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoards
{
    public class UpdateBoardValidatorShould
    {
        private readonly UpdateBoardValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new UpdateBoardCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                Name = "Test board",
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(UpdateBoardCommand command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new UpdateBoardCommand
            {
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                Name = "Test board",
            };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { Name = "" } };
            yield return new object[] { validCommand with { Name = null } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty, Name = null } };
        }
    }
}
