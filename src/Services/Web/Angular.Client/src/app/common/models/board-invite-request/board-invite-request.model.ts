export class BoardInviteRequestModel {
    constructor(
        public Id: string,
        public BoardId: string,
        public BoardName: string,
        public FromAccountId: string,
        public FromAccountName: string,
        public ToAccountId: string,
        public ToAccountName: string,
        public ToAccountEmail: string,
        public CreatedAt: Date
    ) { }
}