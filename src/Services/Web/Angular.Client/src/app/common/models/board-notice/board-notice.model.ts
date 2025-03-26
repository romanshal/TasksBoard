export class BoardNoticeModel {
    constructor(
        public AuthorId: string,
        public BoardId: string,
        public BoardName: string,
        public Definition: string,
        public NoticeStatusName: string
    ){}
}