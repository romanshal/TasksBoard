﻿using FluentAssertions;
using TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard;
using TasksBoard.Application.Validators.ManageBoards;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoards
{
    public class DeleteBoardValidatorShould
    {
        private readonly DeleteBoardValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new DeleteBoardCommand
            {
                Id = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(DeleteBoardCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new DeleteBoardCommand
            {
                Id = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037")
            };
            yield return new object[] { validCommand with { Id = Guid.Empty } };
        }
    }
}
