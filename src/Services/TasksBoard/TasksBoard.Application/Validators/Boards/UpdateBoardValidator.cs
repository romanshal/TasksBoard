using FluentValidation;
using TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.API.Validators.Boards
{
    public class UpdateBoardValidator : AbstractValidator<UpdateBoardCommand>
    {
        public UpdateBoardValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage(BoardMessages.BoardNameRequired);
        }
    }
}
