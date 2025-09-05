import { BoardMemberPermission } from "../board-member-permission/board-member-permission.model";

export class BoardMemberModel {
        public id!: string;
        public boardId!: string;
        public accountId!: string;
        public isOwner!: Boolean;
        public nickname!: string;
        public createdAt!: Date;
        public permissions!: BoardMemberPermission[];
}