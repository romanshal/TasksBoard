import { animate, style, transition, trigger } from '@angular/animations';
import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BoardNoticeService } from '../../services/board-notice/board-notice.service';
import { SessionStorageService } from '../../services/session-storage/session-storage.service';
import { BoardNoticeModel } from '../../models/board-notice/board-notice.model';
import { UserService } from '../../services/user/user.service';
import { UserInfoModel } from '../../models/user/user-info.model';
import { BoardMemberAuthService } from '../../services/board-member-auth/board-member-auth.service';
import { AuthStateService } from '../../services/auth-state/auth-state.service';

interface NoteStyle {
  value: any;
  name?: string;
}

@Component({
  selector: 'app-board-notice.modal',
  standalone: false,
  templateUrl: './board-notice.modal.html',
  styleUrl: './board-notice.modal.scss',
  animations: [
    trigger('modalContainerAnimation', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('200ms ease-out', style({ opacity: 1 }))
      ]),
      transition(':leave', [
        animate('200ms ease-in', style({ opacity: 0 }))
      ])
    ]),
    trigger('modalContentAnimation', [
      transition(':enter', [
        style({ transform: 'scale(0.8)', opacity: 0 }),
        animate('200ms ease-out', style({ transform: 'scale(1)', opacity: 1 }))
      ]),
      transition(':leave', [
        animate('200ms ease-in', style({ transform: 'scale(0.8)', opacity: 0 }))
      ])
    ])
  ]
})
export class BoardNoticeModal implements OnInit {
  form!: FormGroup;
  authorId: string;
  authorName: string;
  author?: UserInfoModel;
  private closeStatus = 'close';
  private successStatus = 'success';

  note?: BoardNoticeModel;
  public disabledActions = false;
  public backgroundColor!: NoteStyle;
  private rotation?: string;

  currentUser?: UserInfoModel | null;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<BoardNoticeModal>,
    private noticeService: BoardNoticeService,
    private userService: UserService,
    private authStateService: AuthStateService,
    private boardMemberAuthService: BoardMemberAuthService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string, note?: BoardNoticeModel }
  ) {
    this.note = data.note;
    this.authStateService.currentUser$.subscribe(user => {
        this.currentUser = user;
    })

    if (data.note !== undefined) {
      this.disabledActions = true;
      this.backgroundColor = this.colors.find(color => color.value === this.note?.BackgroundColor)!;
      this.rotation = this.data.note?.Rotation;
      this.authorId = this.data.note?.AuthorId!;
      this.authorName = this.data.note?.AuthorName!;

      this.userService.getUserInfo(this.authorId).subscribe((result) => {
        this.author = result;
      })
    } else {
      this.backgroundColor = this.getRandomColor();
      this.rotation = this.getRandomRotation();
      this.authorId = this.currentUser!.Id;
      this.authorName = this.currentUser!.Username;
    }
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.form = this.fb.group({
      authorId: [this.authorId, [Validators.required]],
      authorName: [this.authorName, [Validators.required]],
      noticeId: [this.note?.Id],
      definition: [{ value: this.note?.Definition, disabled: this.disabledActions }, Validators.required],
      backgroundColor: [this.backgroundColor.value, Validators.required],
      rotation: [this.rotation, Validators.required]
    });
  }

  havePermission(permission: string){
    return this.boardMemberAuthService.havePermission(permission);
  }

  submitForm() {
    if (this.form.valid) {
      if (this.note === undefined) {
        this.createNotice();
      } else {
        this.updateNotice();
      }
    } else {
      console.error('Форма недействительна!');
    }
  }

  updateStatus(comlete: boolean) {
    if (!this.note) {
      return;
    }

    let body = {
      accountId: this.currentUser!.Id,
      accountName: this.currentUser!.Username,
      noticeId: this.note.Id,
      complete: comlete
    }

    this.noticeService.updateBoardNoticeStatus(this.data.boardId, body).subscribe(result => {
      if (result.IsError || result.Description !== undefined) {
        return;
      }

      this.closeModal(this.successStatus);
    });
  }

  delete() {
    if (!this.note) {
      return;
    }

    this.noticeService.deleteBoardNotice(this.data.boardId, this.note.Id).subscribe(result => {
      if (result.IsError || result.Description !== undefined) {
        return;
      }

      this.closeModal(this.successStatus);
    })
  }

  private createNotice() {
    this.noticeService.createBoardNotice(this.data.boardId, this.form.value).subscribe((result) => {
      console.log(result);
      if (result.IsError || result.Description !== undefined) {
        return;
      }

      this.closeModal(this.successStatus);
    });
  }

  private updateNotice() {
    this.noticeService.updateBoardNotice(this.data.boardId, this.form.value).subscribe((result) => {
      console.log(result);
      if (result.IsError || result.Description !== undefined) {
        return;
      }

      this.closeModal(this.successStatus);
    });
  }

  closeModal(result?: string): void {
    this.form.reset();

    if (result === undefined) {
      this.dialogRef.close(this.closeStatus);
    }

    this.dialogRef.close(result);
  }

  onContainerClick(event: MouseEvent): void {
    // Если кликнули по затемнённой области вне модального окна
    if ((event.target as HTMLElement).classList.contains('modal-container')) {
      this.closeModal(this.closeStatus);
    }
  }

  // Закрытие модального окна по клавише ESC
  @HostListener('document:keydown.escape', ['$event'])
  onEscapePress(event: KeyboardEvent) {
    this.closeModal(this.closeStatus);
  }

  private getRandomColor(): NoteStyle {
    const randomIndex = Math.floor(Math.random() * this.colors.length);
    return this.colors[randomIndex];
  }

  private getRandomRotation() {
    const randomIndex = Math.floor(Math.random() * this.rotations.length);
    return this.rotations[randomIndex].value;
  }

  showActions() {
    this.disabledActions = false;
    this.form.controls['definition'].enable();
  }

  colors: NoteStyle[] = [
    { value: '#fffcab', name: 'Yellow' }, // Желтый
    { value: '#ffd8d5', name: 'Pink' }, // Розово-красный
    { value: '#c8eeff', name: 'Blue' }, // Голубой
    { value: '#e4fcc7', name: 'Green' },  // Светло-зеленый
    { value: '#e8dff7', name: 'Lavender'}, //  Лавандовый
    { value: '#fff4d4', name: 'Cream'}, // Кремовый
  ];

  isOpenColors = false;
  toggleDropdownColors(event: Event) {
    event.stopPropagation();
    this.isOpenColors = !this.isOpenColors;
  }

  selectColor(color: NoteStyle) {
    this.backgroundColor = color;
    this.isOpenColors = false;
  }

  rotations: NoteStyle[] = [
    { value: '6deg' },
    { value: '5.5deg' },
    { value: '5deg' },
    { value: '4.5deg' },
    { value: '4deg' },
    { value: '3.5deg' },
    { value: '3deg' },
    { value: '2.5deg' },
    { value: '2deg' },

    { value: '-2deg' },
    { value: '-2.5deg' },
    { value: '-3deg' },
    { value: '-3.5deg' },
    { value: '-4deg' },
    { value: '-4.5deg' },
    { value: '-5deg' },
    { value: '-5.5deg' },
    { value: '-6deg' },
  ]
}
