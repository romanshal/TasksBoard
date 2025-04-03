export class MessageModel {
    constructor(
        public UserId: string,
        public Username: string,
        public MessageDate: Date,
        public Message: string
    ) {}
}