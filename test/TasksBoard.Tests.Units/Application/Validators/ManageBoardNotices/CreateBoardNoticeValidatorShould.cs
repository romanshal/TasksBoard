using FluentAssertions;
using System;
using System.Collections.Generic;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Application.Validators.ManageBoardNotices;

namespace TasksBoard.Tests.Units.Application.Validators.ManageBoardNotices
{
    public class CreateBoardNoticeValidatorShould
    {
        private readonly CreateBoardNoticeValidator _sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandValid()
        {
            var validCommand = new CreateBoardNoticeCommand
            {
                AuthorId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AuthorName = "Test author",
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                Definition = "Test definition",
                BackgroundColor = "#111111",
                Rotation = "100deg"
            };

            _sut.Validate(validCommand).IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandInvalid(CreateBoardNoticeCommand command)
        {
            _sut.Validate(command).IsValid.Should().BeFalse();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new CreateBoardNoticeCommand
            {
                AuthorId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                AuthorName = "Test author",
                BoardId = Guid.Parse("3f73ccb5-1ae0-4752-8803-f6e502bd1037"),
                Definition = "Test definition",
                BackgroundColor = "Test color",
                Rotation = "100deg"
            };
            yield return new object[] { validCommand with { AuthorId = Guid.Empty } };
            yield return new object[] { validCommand with { AuthorName = "" } };
            yield return new object[] { validCommand with { AuthorName = null } };
            yield return new object[] { validCommand with { BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { Definition = "" } };
            yield return new object[] { validCommand with { Definition = null } };
            yield return new object[] { validCommand with { BackgroundColor = "" } };
            yield return new object[] { validCommand with { BackgroundColor = "asdasd" } };
            yield return new object[] { validCommand with { BackgroundColor = null } };
            yield return new object[] { validCommand with { Rotation = "" } };
            yield return new object[] { validCommand with { Rotation = "adasda" } };
            yield return new object[] { validCommand with { Rotation = null } };
            yield return new object[] { validCommand with { AuthorId = Guid.Empty, AuthorName = null } };
            yield return new object[] { validCommand with { AuthorId = Guid.Empty, AuthorName = null, BoardId = Guid.Empty } };
            yield return new object[] { validCommand with { AuthorId = Guid.Empty, AuthorName = null, BoardId = Guid.Empty, Definition = null } };
            yield return new object[] { validCommand with { AuthorId = Guid.Empty, AuthorName = null, BoardId = Guid.Empty, Definition = null, BackgroundColor = null } };
            yield return new object[] { validCommand with { AuthorId = Guid.Empty, AuthorName = null, BoardId = Guid.Empty, Definition = null, BackgroundColor = null, Rotation = null } };
        }
    }
}
