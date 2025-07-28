using FluentValidation;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.BoardNotices
{
    public class GetPaginatedListValidator : AbstractValidator<GetPaginatedListQuery<BoardNoticeDto>>
    {
        public GetPaginatedListValidator()
        {
            RuleFor(p => p.PageIndex)
                .GreaterThan(0)
                .WithMessage(BoardNoticeMessages.PageIndexLessZero);

            RuleFor(p => p.PageSize)
                .GreaterThan(0)
                .WithMessage(BoardNoticeMessages.PageSizeLessZero);
        }
    }
}
