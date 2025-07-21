using FluentValidation;
using TasksBoard.Application.Features.BoardMembers.Queries.GetPaginatedBoardMembersByBoardId;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.BoardMembers
{
    public class GetPaginatedBoardMembersByBoardIdValidator : AbstractValidator<GetPaginatedBoardMembersByBoardIdQuery>
    {
        public GetPaginatedBoardMembersByBoardIdValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.BoardIdRequired);

            RuleFor(p => p.PageIndex)
                .GreaterThan(0)
                .WithMessage(BoardMemberMessages.PageIndexLessZero);

            RuleFor(p => p.PageSize)
                .GreaterThan(0)
                .WithMessage(BoardMemberMessages.PageSizeLessZero);
        }
    }
}
