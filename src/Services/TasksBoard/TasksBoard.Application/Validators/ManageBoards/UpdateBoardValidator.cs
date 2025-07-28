using FluentValidation;
using TasksBoard.Application.Features.ManageBoards.Commands.UpdateBoard;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.ManageBoards
{
    public class UpdateBoardValidator : AbstractValidator<UpdateBoardCommand>
    {
        public UpdateBoardValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessages.BoardIdRequired);

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardMessages.BoardNameRequired);
        }
    }
}
