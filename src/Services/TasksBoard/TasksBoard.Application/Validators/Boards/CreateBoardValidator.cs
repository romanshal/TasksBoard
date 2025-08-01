﻿using FluentValidation;
using TasksBoard.Application.Features.Boards.Commands.CreateBoard;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.Boards
{
    public class CreateBoardValidator : AbstractValidator<CreateBoardCommand>
    {
        public CreateBoardValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardMessages.BoardNameRequired);

            RuleFor(x => x.OwnerId)
                .NotNull()
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMessages.BoardOwnerRequired);

            RuleFor(x => x.OwnerNickname)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardMessages.BoardOwnerNicknameRequired);
        }
    }
}
