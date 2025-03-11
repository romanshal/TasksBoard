using FluentValidation;
using TasksBoard.API.Constants.Messages;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;

namespace TasksBoard.API.Validators.Boards
{
    public class CreateBoardValidator : AbstractValidator<CreateBoardCommand>
    {
        public CreateBoardValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage(BoardMessages.BoardNameRequired);

            RuleFor(x => x.OwnerId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardMessages.BoardOwnerRequired);
        }
    }
}
