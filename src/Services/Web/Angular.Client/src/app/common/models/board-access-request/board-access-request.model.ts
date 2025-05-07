export class BoardAccessRequestModel {
    constructor(
        public Id: string,
        public BoardId: string,
        public BoardName: string,
        public AccountId: string,
        public AccountName: string,
        public AccountEmail: string,
        public CreatedAt: Date
    ) { }
}