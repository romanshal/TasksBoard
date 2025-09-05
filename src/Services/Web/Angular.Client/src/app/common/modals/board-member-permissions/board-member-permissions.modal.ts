import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardPermission } from '../../models/board-permission/board-permission.model';
import { ManageBoardMemberService } from '../../services/manage-board-member/manage-board-member.service';
import { UserService } from '../../services/user/user.service';
import { UserInfoModel } from '../../models/user/user-info.model';
import { DeleteConfirmationModal } from '../delete-confirmation/delete-confirmation.modal';
import { Observable } from 'rxjs';

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
  userAvatar!: Observable<string>;

  permissions!: BoardPermission[];

  updatedStatus = 'updated';

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<BoardMemberPermissionsModal>,
    private manageBoardMemberService: ManageBoardMemberService,
    private userService: UserService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string, member: BoardMemberModel, permissions: BoardPermission[], userAvatar: Observable<string> }
  ) {
    this.boardId = data.boardId;
    this.member = data.member;
    this.userAvatar = data.userAvatar;
    this.permissions = data.permissions;

    this.userService.getUserInfo(this.member.accountId).subscribe(result => {
      if (result) {
        this.user = result;
      }
    });
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.form = this.fb.group({
      memberId: [this.member.id, [Validators.required]],
      permissions: this.fb.array(this.member.permissions.map(t => this.fb.control(t.boardPermissionId)))
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
  }

  isPermissionsChecked(permissionId: string) {
    if (this.member.permissions.findIndex(p => p.boardPermissionId === permissionId) !== -1) {
      return true;
    } else {
      return false;
    }
  }

  savePermissions() {
    if (this.form.valid) {
      this.manageBoardMemberService.updateMemberPermissions(this.boardId, this.form.value).subscribe({
        next: () => {
          this.closeModal(this.updatedStatus);
        },
        error: () => {
          
        }
      });
    }
  }

  openDeleteMember() {
    this.dialog.open(DeleteConfirmationModal, {
      data: {
        element: 'member',
        elementName: this.member.nickname,
        secondConfirme: false
      }
    }).afterClosed().subscribe(result => {
      if (result === 'confirmed') {
        this.manageBoardMemberService.deleteMember(this.boardId, this.member.id).subscribe({
          next: () => {
            this.closeModal(this.updatedStatus)
          },
          error: err => {

          }
        })
      }
    });
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
