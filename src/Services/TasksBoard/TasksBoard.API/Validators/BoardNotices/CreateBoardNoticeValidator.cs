using FluentValidation;
using TasksBoard.API.Constants.Messages;
using TasksBoard.Application.Models.Requests.ManageBoardNotices;

namespace TasksBoard.API.Validators.BoardNotices
{
    public class CreateBoardNoticeValidator : AbstractValidator<CreateBoardNoticeRequest>
    {
        public CreateBoardNoticeValidator()
        {
            RuleFor(x => x.AuthorId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.AuthorIdRequired);

            RuleFor(x => x.Definition)
                .NotNull().NotEmpty().WithMessage(BoardNoticeMessages.DefinitionRequired);
        }
    }
}
