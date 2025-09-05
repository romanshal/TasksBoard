import { BoardAccessRequestModel } from "../board-access-request/board-access-request.model";
import { BoardInviteRequestModel } from "../board-invite-request/board-invite-request.model";
import { BoardMemberModel } from "../board-member/board-member.model";

export class BoardModel {
        public id!: string;
        public ownerId!: string;
        public name!: string;
        public description?: string;
        public tags?: string[];
        public isPublic!: boolean;
        public image?: string;
        public imageExtension?: string;
        public members!: BoardMemberModel[];
        public accessRequests?: BoardAccessRequestModel[]
        public inviteRequests?: BoardInviteRequestModel[]
}