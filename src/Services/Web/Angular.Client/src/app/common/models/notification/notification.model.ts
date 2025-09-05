export class NotificationModel {
    constructor(
        public id: string,
        public type: string,
        public payload: Map<string, string>,
        public read: boolean,
        public createdAt: Date
    ){}
}