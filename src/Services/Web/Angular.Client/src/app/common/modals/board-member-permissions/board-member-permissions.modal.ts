import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardPermission } from '../../models/board-permission/board-permission.model';
import { ManageBoardMemberService } from '../../services/manage-board-member/manage-board-member.service';
import { UserService } from '../../services/user/user.service';
import { UserInfoModel } from '../../models/user/user-info.model';

@Component({
  selector: 'app-board-member-permissions',
  standalone: false,
  templateUrl: './board-member-permissions.modal.html',
  styleUrl: './board-member-permissions.modal.scss'
})
export class BoardMemberPermissionsModal implements OnInit {
  form!: FormGroup;

  boardId!: string;

  member!: BoardMemberModel;
  user?: UserInfoModel;

  permissions!: BoardPermission[];

  updatedStatus = 'updated';

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<BoardMemberPermissionsModal>,
    private manageBoardMemberService: ManageBoardMemberService,
    private userService: UserService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string, member: BoardMemberModel, permissions: BoardPermission[] }
  ) { 
    this.boardId = data.boardId;
    this.member = data.member;
    this.permissions = data.permissions;

    this.userService.getUserInfo(this.member.AccountId).subscribe(result => {
      if(result){
        this.user = result;
      }
    });
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.form = this.fb.group({
      memberId: [this.member.Id, [Validators.required]],
      permissions: this.fb.array(this.member.Permissions.map(t => this.fb.control(t.BoardPermissionId)))
    });
  }

  get getPermissions(): FormArray {
    return this.form.get('permissions') as FormArray;
  }

  checkPermission(permissionId: string) {
    const index = this.getPermissions.controls.findIndex(control => control.value === permissionId);
    if (index !== -1) {
      this.getPermissions.removeAt(index);
    } else {
      this.getPermissions.push(this.fb.control(permissionId));
    }

    console.log(this.getPermissions.controls);
  }

  isPermissionsChecked(permissionId: string) {
    if (this.member.Permissions.findIndex(p => p.BoardPermissionId === permissionId) !== -1) {
      return true;
    } else {
      return false;
    }
  }

  savePermissions() {
    if (this.form.valid) {
      this.manageBoardMemberService.updateMemberPermissions(this.boardId, this.form.value).subscribe(result => {
        if(result){
          this.closeModal(this.updatedStatus);
        }
      });
    }
  }

  onContainerClick(event: MouseEvent): void {
    // Если кликнули по затемнённой области вне модального окна
    if ((event.target as HTMLElement).classList.contains('modal-container')) {
      this.closeModal();
    }
  }

  // Закрытие модального окна по клавише ESC
  @HostListener('document:keydown.escape', ['$event'])
  onEscapePress(event: KeyboardEvent) {
    this.closeModal();
  }

  closeModal(result?: string): void {
    this.dialogRef.close(result);
  }
}
