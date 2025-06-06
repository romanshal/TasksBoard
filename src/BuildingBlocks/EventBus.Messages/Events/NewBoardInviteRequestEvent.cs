﻿namespace EventBus.Messages.Events
{
    public class NewBoardInviteRequestEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }        
        public required Guid AccountId { get; set; }
        public required Guid FromAccountId { get; set; }
        public required string FromAccountName { get; set; }
    }
}
