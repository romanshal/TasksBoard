using FluentValidation;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByBoardId;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.BoardNotices
{
    public class GetPaginatedBoardNoticesByBoardIdValidator : AbstractValidator<GetPaginatedBoardNoticesByBoardIdQuery>
    {
        public GetPaginatedBoardNoticesByBoardIdValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardIdRequired);

            RuleFor(p => p.PageIndex)
                .GreaterThan(0)
                .WithMessage(BoardMemberMessages.PageIndexLessZero);

            RuleFor(p => p.PageSize)
                .GreaterThan(0)
                .WithMessage(BoardMemberMessages.PageSizeLessZero);
        }
    }
}
