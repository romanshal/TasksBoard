using Chat.Application.Features.BoardMessages.Commands.DeleteBoardMessage;
using Chat.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Chat.Application.Validators.BoardMessages
{
    public class DeleteBoardMessageValidator : AbstractValidator<DeleteBoardMessageCommand>
    {
        public DeleteBoardMessageValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessageMessages.BoardIdRequired);

            RuleFor(p => p.BoardMessageId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessageMessages.MessageIdRequired);
        }
    }
}
