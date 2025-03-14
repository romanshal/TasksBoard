using FluentValidation;
using TasksBoard.API.Constants.Messages;
using TasksBoard.Application.Models.Requests.ManageBoards;

namespace TasksBoard.API.Validators.Boards
{
    public class UpdateBoardValidator : AbstractValidator<UpdateBoardRequest>
    {
        public UpdateBoardValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().NotEmpty().WithMessage(BoardMessages.BoardNameRequired);
        }
    }
}
