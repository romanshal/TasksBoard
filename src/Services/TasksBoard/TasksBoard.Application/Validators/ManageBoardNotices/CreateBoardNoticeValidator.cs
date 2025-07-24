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

            RuleFor(x => x.AuthorName)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardNoticeMessages.AuthorNameRequired);

            RuleFor(x => x.BoardId)
                .NotNull()
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardIdRequired);

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
