import { BoardAccessRequestModel } from "../board-access-request/board-access-request.model";
import { BoardMemberModel } from "../board-member/board-member.model";

export class BoardModel {
        public Id!: string;
        public OwnerId!: string;
        public Name!: string;
        public Description?: string;
        public Tags?: string[];
        public Public!: boolean;
        public Image?: string;
        public ImageExtension?: string;
        public Members!: BoardMemberModel[];
        public AccessRequests?: BoardAccessRequestModel[]
}