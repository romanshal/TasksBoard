export class BoardInviteRequestModel {
    constructor(
        public id: string,
        public boardId: string,
        public boardName: string,
        public fromAccountId: string,
        public fromAccountName: string,
        public toAccountId: string,
        public toAccountName: string,
        public toAccountEmail: string,
        public createdAt: Date
    ) { }
}