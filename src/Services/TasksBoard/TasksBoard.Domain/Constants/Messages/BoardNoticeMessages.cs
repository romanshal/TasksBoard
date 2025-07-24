namespace TasksBoard.Domain.Constants.Messages
{
    public class BoardNoticeMessages : BaseMessages
    {
        public const string BoardNoticeIdRequired = "Notice id is required.";
        public const string AuthorIdRequired = "Notice author is required.";
        public const string AuthorNameRequired = "Notice author name is required.";
        public const string StatusIdRequired = "Notice status is required.";
        public const string DefinitionRequired = "Definition is required.";
        public const string BackgroundColorRequired = "Background is required.";
        public const string RotationRequired = "Rotation is required.";
        public const string PageIndexLessZero = "Page index must be greater than zero.";
        public const string PageSizeLessZero = "Page size must be greater than zero.";
    }
}
