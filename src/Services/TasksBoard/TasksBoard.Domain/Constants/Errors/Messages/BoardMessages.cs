namespace TasksBoard.Domain.Constants.Errors.Messages
{
    public class BoardMessages
    {
        public static string NotFound(Guid id) => $"Board with id '{id}' was not found.";
    }
}
