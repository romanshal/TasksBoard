using FluentValidation;
using TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.ManageBoards
{
    public class DeleteBoardValidator : AbstractValidator<DeleteBoardCommand>
    {
        public DeleteBoardValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessages.BoardIdRequired);
        }
    }
}
