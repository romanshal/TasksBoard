import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-operation-result-message',
  standalone: false,
  templateUrl: './operation-result-message.modal.html',
  styleUrl: './operation-result-message.modal.scss'
})
export class OperationResultMessageModal {
  isSuccess!: boolean;
  message!: string;

  constructor(
    private dialogRef: MatDialogRef<OperationResultMessageModal>,
    @Inject(MAT_DIALOG_DATA) private data: { isSuccess: boolean, message: string }
  ) { 
    this.isSuccess = data.isSuccess;
    this.message = data.message;
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
  
    closeModal(): void {
      this.dialogRef.close();
    }
}
