using FluentValidation;
using TasksBoard.Application.Features.BoardInvites.Commands.ResolveInviteRequest;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.BoardInvites
{
    public class ResolveInviteRequestValidator : AbstractValidator<ResolveInviteRequestCommand>
    {
        public ResolveInviteRequestValidator()
        {
            RuleFor(p => p.RequestId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardInviteMessages.RequestIdRequired);

            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardInviteMessages.BoardIdRequired);
        }
    }
}
