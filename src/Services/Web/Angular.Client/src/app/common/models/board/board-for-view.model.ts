export class BoardForViewModel {
    constructor(
        public Id: string,
        public Name: string,
        public Description: string,
        public Tags: string[],
        public MemberCount: number,
        public IsMember: boolean,
        public Public : boolean,
        public Image: string,
        public ImageExtension: string
    ){}
}