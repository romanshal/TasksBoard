using FluentValidation;
using TasksBoard.Application.Features.Boards.Queries.GetBoardById;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.Boards
{
    public class GetBoardByIdValidator : AbstractValidator<GetBoardByIdQuery>
    {
        public GetBoardByIdValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessages.BoardIdRequired);
        }
    }
}
