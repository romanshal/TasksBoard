using FluentValidation;
using TasksBoard.API.Constants.Messages;
using TasksBoard.Application.Features.Boards.Commands.UpdateBoard;

namespace TasksBoard.API.Validators.Boards
{
    public class UpdateBoardValidator : AbstractValidator<UpdateBoardCommand>
    {
        public UpdateBoardValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage(BoardMessages.BoardNameRequired);

            RuleFor(x => x.Id)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardMessages.BoardIdRequired);
        }
    }
}
