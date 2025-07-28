using Chat.Application.Features.BoardMessages.Queries.GetBoardMessagesByBoardId;
using Chat.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Chat.Application.Validators.BoardMessages
{
    public class GetBoardMessagesByBoardIdValidator : AbstractValidator<GetBoardMessagesByBoardIdQuery>
    {
        public GetBoardMessagesByBoardIdValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessageMessages.BoardIdRequired);

            RuleFor(p => p.PageIndex)
                .GreaterThan(0)
                .WithMessage(BoardMessageMessages.PageIndexLessZero);

            RuleFor(p => p.PageSize)
                .GreaterThan(0)
                .WithMessage(BoardMessageMessages.PageSizeLessZero);
        }
    }
}
