export class NotificationModel {
    constructor(
        public Id: string,
        public Type: string,
        public Payload: Map<string, string>,
        public Read: boolean,
        public CreatedAt: Date
    ){}
}