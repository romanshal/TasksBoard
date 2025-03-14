using FluentValidation;
using TasksBoard.API.Constants.Messages;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;

namespace TasksBoard.API.Validators.BoardNotices
{
    public class CreateBoardNoticeValidator : AbstractValidator<CreateBoardNoticeCommand>
    {
        public CreateBoardNoticeValidator()
        {
            RuleFor(x => x.AuthorId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.AuthorIdRequired);

            RuleFor(x => x.BoardId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.BoardIdRequired);

            RuleFor(x => x.NoticeStatusId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.StatusIdRequired);

            RuleFor(x => x.Definition)
                .NotNull().NotEmpty().WithMessage(BoardNoticeMessages.DefinitionRequired);
        }
    }
}
