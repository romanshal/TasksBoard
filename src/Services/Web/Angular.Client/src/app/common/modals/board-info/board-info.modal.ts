import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { BoardModel } from '../../models/board/board.model';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-board-info',
  standalone: false,
  templateUrl: './board-info.modal.html',
  styleUrl: './board-info.modal.scss'
})
export class BoardInfoModal implements OnInit {
  board!: BoardModel;
  form!: FormGroup;

  isOwner = false;

  public disabledActions = true;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<BoardInfoModal>,
    @Inject(MAT_DIALOG_DATA) private data: { board: BoardModel, isOwner: boolean }
  ) {
    this.board = data.board;
    this.isOwner = data.isOwner;
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.form = this.fb.group({
      name: [{ value: this.board.Name, disabled: this.disabledActions }, Validators.required],
      description: [{ value: this.board.Description, disabled: this.disabledActions }, Validators.required],
    });
  }

  showActions() {
    this.disabledActions = false;
    this.form.controls['name'].enable();
    this.form.controls['description'].enable();
  }

  submitForm() {
    if (this.form.valid) {

    } else {
      console.error('Форма недействительна!');
    }
  }

  closeModal(result?: string): void {
    this.dialogRef.close();
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
}
