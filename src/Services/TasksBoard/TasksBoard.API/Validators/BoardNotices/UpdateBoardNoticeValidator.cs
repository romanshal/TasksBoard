using FluentValidation;
using TasksBoard.API.Constants.Messages;
using TasksBoard.Application.Models.Requests.ManageBoardNotices;

namespace TasksBoard.API.Validators.BoardNotices
{
    public class UpdateBoardNoticeValidator : AbstractValidator<UpdateBoardNoticeRequest>
    {
        public UpdateBoardNoticeValidator()
        {
            RuleFor(x => x.NoticeId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.BoardNoticeIdRequired);

            RuleFor(x => x.NoticeStatusId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.StatusIdRequired);

            RuleFor(x => x.Definition)
                .NotNull().NotEmpty().WithMessage(BoardNoticeMessages.DefinitionRequired);
        }
    }
}
