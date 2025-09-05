export class BoardAccessRequestModel {
    constructor(
        public id: string,
        public boardId: string,
        public boardName: string,
        public accountId: string,
        public accountName: string,
        public accountEmail: string,
        public createdAt: Date
    ) { }
}