import { animate, style, transition, trigger } from '@angular/animations';
import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BoardNoticeService } from '../../services/board-notice/board-notice.service';
import { SessionStorageService } from '../../services/session-storage/session-storage.service';
import { HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-add-board-notice.modal',
  standalone: false,
  templateUrl: './add-board-notice.modal.component.html',
  styleUrl: './add-board-notice.modal.component.scss',
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
export class AddBoardNoticeModalComponent implements OnInit {
  form!: FormGroup;
  private authorId!: string;
  private closeStatus = 'close';
  private successStatus = 'success';

  private isDisabled = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddBoardNoticeModalComponent>,
    private noticeService: BoardNoticeService,
    private sessionService: SessionStorageService,
    @Inject(MAT_DIALOG_DATA) private data: { borderId: string, disabled: boolean, content?: string }
  ) {
    this.isDisabled = data.disabled;
   }

  ngOnInit(): void {
    this.authorId = this.sessionService.getItem(this.sessionService.userIdKey)!;
    this.initForm();
  }

  initForm() {
    this.form = this.fb.group({
      authorId: [this.authorId, [Validators.required]],
      definition: [{ value: this.data.content, disabled: this.isDisabled }, Validators.required],
      noticeStatusId: ['a3372135-ea3d-4eb9-8209-5a36634b2bba', Validators.required]
    });
  }

  submitForm() {
    console.log(this.form.value);
    if (this.form.valid) {
      this.noticeService.createBoardNotice(this.data.borderId, this.form.value).subscribe((result) => {
        console.log(result);
        if (result.IsError || result.Description !== null) {
          this.form.reset();

          this.closeModal(this.successStatus);
        }
      })
    } else {
      console.error('Форма недействительна!');
    }
  }

  closeModal(result: string): void {
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
}
