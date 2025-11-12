using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Application.Validators.Boards;

namespace TasksBoard.Tests.Units.Application.Validators.Boards
{
    public class CreateBoardValidatorShould
    {
        private readonly CreateBoardValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new CreateBoardCommand
            {
                OwnerId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                OwnerNickname = "Test nickname",
                Name = "Test board",
            };

            sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(CreateBoardCommand command)
        {
            sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new CreateBoardCommand
            {
                OwnerId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                OwnerNickname = "Test nickname",
                Name = "Test board",
            };
            yield return new object[] { validCommand with { OwnerId = Guid.Empty } };
            yield return new object[] { validCommand with { OwnerNickname = string.Empty } };
            yield return new object[] { validCommand with { OwnerNickname = "" } };
            yield return new object[] { validCommand with { Name = string.Empty } };
            yield return new object[] { validCommand with { Name = "" } };
            yield return new object[] { validCommand with { OwnerId = Guid.Empty, OwnerNickname = "" } };
            yield return new object[] { validCommand with { OwnerId = Guid.Empty, Name = "" } };
            yield return new object[] { validCommand with { OwnerNickname = null, Name = null } };
            yield return new object[] { validCommand with { OwnerId = Guid.Empty, OwnerNickname = "", Name = null } };

        }
    }
}
