using FluentValidation;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.ManageBoardNotices
{
    public class CreateBoardNoticeValidator : AbstractValidator<CreateBoardNoticeCommand>
    {
        public CreateBoardNoticeValidator()
        {
            RuleFor(x => x.AuthorId)
                .NotNull()
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.AuthorIdRequired);

            RuleFor(x => x.Definition)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardNoticeMessages.DefinitionRequired);
        }
    }
}
