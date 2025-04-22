import { BoardMemberPermission } from "../board-member-permission/board-member-permission.model";

export class BoardMemberModel {
        public Id!: string;
        public BoardId!: string;
        public AccountId!: string;
        public IsOwner!: Boolean;
        public Nickname!: string;
        public CreatedAt!: Date;
        public Permissions!: BoardMemberPermission[];
}