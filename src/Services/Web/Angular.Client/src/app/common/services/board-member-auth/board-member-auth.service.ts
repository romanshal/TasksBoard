import { Injectable } from '@angular/core';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardModel } from '../../models/board/board.model';

@Injectable({
  providedIn: 'root'
})
export class BoardMemberAuthService {
  currentBoard!: BoardModel;
  currentMember!: BoardMemberModel;
  currentMemberPermission: string[] = [];

  constructor() { }

  initialize(currentBoard: BoardModel, currentMember: BoardMemberModel) {
    this.currentBoard = currentBoard;
    this.currentMember = currentMember;
    this.currentMemberPermission = this.currentMember.Permissions.map(perm => perm.BoardPermissionName);
  }

  havePermission(permission: string){
    return this.currentMemberPermission.find(perm => perm === permission) !== undefined;
  }
}
