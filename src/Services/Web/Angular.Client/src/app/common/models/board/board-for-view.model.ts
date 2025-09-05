export class BoardForViewModel {
    constructor(
        public id: string,
        public name: string,
        public description: string,
        public tags: string[],
        public memberCount: number,
        public isMember: boolean,
        public isPublic : boolean,
        public image: string,
        public imageExtension: string
    ){}
}