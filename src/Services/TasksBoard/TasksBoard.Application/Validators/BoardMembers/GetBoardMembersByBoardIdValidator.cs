using FluentValidation;
using TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMembersByBoardId;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.BoardMembers
{
    public class GetBoardMembersByBoardIdValidator : AbstractValidator<GetBoardMembersByBoardIdQuery>
    {
        public GetBoardMembersByBoardIdValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.BoardIdRequired);
        }
    }
}
