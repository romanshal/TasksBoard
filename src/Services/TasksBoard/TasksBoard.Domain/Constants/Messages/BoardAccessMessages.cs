using MassTransit;

namespace TasksBoard.Domain.Constants.Messages
{
    public class BoardAccessMessages : BaseMessages
    {
        public static string BoardAccessIdRequired = "Board access id is required.";
        public static string AccountEmailInvalid = "Account email is invalid.";
    }
}
