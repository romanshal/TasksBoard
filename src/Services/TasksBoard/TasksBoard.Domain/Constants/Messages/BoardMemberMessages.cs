namespace TasksBoard.Domain.Constants.Messages
{
    public class BoardMemberMessages : BaseMessages
    {
        public static string PageIndexLessZero = "Page index must be greater than zero.";
        public static string PageSizeLessZero = "Page size must be greater than zero.";
        public static string MemberIdRequired = "Member id is required.";
        public static string PermissionsRequired = "Permissions is required.";
        public static string RemoveAccountIdRequired = "Remove account id is required.";
    }
}
