export class BoardMessageModel {
    constructor(
        public id: string,
        public memberId: string,
        public accountId: string,
        public memberNickname: string,
        public message: string,
        public createdAt: Date,
        public modifiedAt: Date,
        public isDeleted: boolean
    ) {}
}