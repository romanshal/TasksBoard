using FluentValidation;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.ManageBoardNotices
{
    public class UpdateBoardNoticeValidator : AbstractValidator<UpdateBoardNoticeCommand>
    {
        public UpdateBoardNoticeValidator()
        {
            RuleFor(x => x.BoardId)
                .NotNull()
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardIdRequired);

            RuleFor(x => x.NoticeId)
                .NotNull()
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardNoticeIdRequired);            
            
            RuleFor(x => x.AccountId)
                .NotNull()
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.AccountIdRequired);

            RuleFor(x => x.Definition)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardNoticeMessages.DefinitionRequired);

            RuleFor(x => x.BackgroundColor)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardNoticeMessages.BackgroundColorRequired);

            RuleFor(x => x.Rotation)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardNoticeMessages.RotationRequired);
        }
    }
}
