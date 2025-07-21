using FluentValidation;
using TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.BoardNotices
{
    public class GetBoardNoticeByIdValidator : AbstractValidator<GetBoardNoticeByIdQuery>
    {
        public GetBoardNoticeByIdValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardNoticeIdRequired);
        }
    }
}
