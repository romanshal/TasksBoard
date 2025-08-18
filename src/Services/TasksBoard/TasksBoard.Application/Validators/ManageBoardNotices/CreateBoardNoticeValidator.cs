using FluentValidation;
using System.Globalization;
using System.Text.RegularExpressions;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.CreateBoardNotice;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.ManageBoardNotices
{
    public partial class CreateBoardNoticeValidator : AbstractValidator<CreateBoardNoticeCommand>
    {
        private static readonly Regex Hex6 = BackgroundColor();
        private static readonly Regex Deg = Rotation();

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
                .WithMessage(BoardNoticeMessages.BackgroundColorRequired)
                .Must(m => m is not null && Hex6.IsMatch(m))
                .WithMessage(BoardNoticeMessages.BackgroundColorInvalid);

            RuleFor(x => x.Rotation)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardNoticeMessages.RotationRequired)
                .Must(m => m is not null && Deg.IsMatch(m))
                .Must(IsRotationInRange)
                .WithMessage(BoardNoticeMessages.RotationInvalid);
        }

        private static bool IsRotationInRange(string? rotation)
        {
            if (string.IsNullOrWhiteSpace(rotation) || !Deg.IsMatch(rotation))
                return false;

            var numPart = rotation[..^3];
            return double.TryParse(numPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                   && value >= -360 && value <= 360;
        }

        [GeneratedRegex(@"^-?\d+(?:\.\d+)?deg$", RegexOptions.Compiled)]
        private static partial Regex Rotation();

        [GeneratedRegex(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled)]
        private static partial Regex BackgroundColor();
    }
}
