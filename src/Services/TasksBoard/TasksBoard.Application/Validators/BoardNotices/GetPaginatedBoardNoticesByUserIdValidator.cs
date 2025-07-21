using FluentValidation;
using TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.BoardNotices
{
    public class GetPaginatedBoardNoticesByUserIdValidator : AbstractValidator<GetPaginatedBoardNoticesByUserIdQuery>
    {
        public GetPaginatedBoardNoticesByUserIdValidator()
        {
            RuleFor(p => p.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.AccountIdRequired);

            RuleFor(p => p.PageIndex)
                .GreaterThan(0)
                .WithMessage(BoardMemberMessages.PageIndexLessZero);

            RuleFor(p => p.PageSize)
                .GreaterThan(0)
                .WithMessage(BoardMemberMessages.PageSizeLessZero);
        }
    }
}
