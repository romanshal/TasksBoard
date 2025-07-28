using Chat.Application.Features.BoardMessages.Commands.CreateBoardMessage;
using Chat.Domain.Constants.Validations.Messages;
using FluentValidation;

namespace Chat.Application.Validators.BoardMessages
{
    public class CreateBoardMessageValidator : AbstractValidator<CreateBoardMessageCommand>
    {
        public CreateBoardMessageValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithErrorCode(BoardMessageMessages.BoardIdRequired);

            RuleFor(p => p.AccountId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessageMessages.AccountIdRequired);

            RuleFor(p => p.MemberId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessageMessages.MemberIdRequired);

            RuleFor(p => p.MemberNickname)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardMessageMessages.NicknameRequired);

            RuleFor(p => p.Message)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardMessageMessages.MessageRequired);

        }
    }
}
