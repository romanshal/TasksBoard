export class BoardMessageModel {
    constructor(
        public Id: string,
        public MemberId: string,
        public AccountId: string,
        public MemberNickname: string,
        public Message: string,
        public CreatedAt: Date,
        public ModifiedAt: Date
    ) {}
}