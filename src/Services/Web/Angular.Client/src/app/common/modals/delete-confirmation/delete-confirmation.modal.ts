import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-delete-confirmation',
  standalone: false,
  templateUrl: './delete-confirmation.modal.html',
  styleUrl: './delete-confirmation.modal.scss'
})
export class DeleteConfirmationModal {
  element: string = '';
  elementName?: string;
  secondConfirme = false;

  confirmed = 'confirmed';

  confirmationString: string = '';
  confirmationError = false;

  constructor(
    private dialogRef: MatDialogRef<DeleteConfirmationModal>,
    @Inject(MAT_DIALOG_DATA) private data: { element: string, secondConfirme: boolean, elementName?: string }
  ) {
    this.element = data.element;
    this.elementName = data.elementName;
    this.secondConfirme = data.secondConfirme;
  }

  inputChange(event: any){
    this.confirmationError = false;
  }

  confirme(){
    if(this.secondConfirme){
      if(this.confirmationString !== this.elementName){
        this.confirmationString = '';
        this.confirmationError = true;
  
        return;
      }
    }

    this.closeModal(this.confirmed);
  }

  closeModal(result?: string): void {
    this.dialogRef.close(result);
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
