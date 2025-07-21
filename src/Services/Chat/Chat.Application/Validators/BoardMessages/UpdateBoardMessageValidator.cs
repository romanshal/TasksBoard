using Chat.Application.Features.BoardMessages.Commands.UpdateBoardMessage;
using Chat.Domain.Constants.Messages;
using FluentValidation;

namespace Chat.Application.Validators.BoardMessages
{
    public class UpdateBoardMessageValidator : AbstractValidator<UpdateBoardMessageCommand>
    {
        public UpdateBoardMessageValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessageMessages.BoardIdRequired);

            RuleFor(p => p.BoardMessageId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessageMessages.MessageIdRequired);

            RuleFor(p => p.Message)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardMessageMessages.MessageRequired);
        }
    }
}
