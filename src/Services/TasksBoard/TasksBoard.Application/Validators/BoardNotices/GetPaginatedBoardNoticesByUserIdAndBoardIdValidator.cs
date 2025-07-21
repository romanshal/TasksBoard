using FluentValidation;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserIdAndBoardId;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.BoardNotices
{
    public class GetPaginatedBoardNoticesByUserIdAndBoardIdValidator : AbstractValidator<GetPaginatedBoardNoticesByUserIdAndBoardIdQuery>
    {
        public GetPaginatedBoardNoticesByUserIdAndBoardIdValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.AccountIdRequired);

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
