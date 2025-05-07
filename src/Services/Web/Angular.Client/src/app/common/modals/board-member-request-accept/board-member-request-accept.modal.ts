import { Component, HostListener, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BoardAccessRequestModel } from '../../models/board-access-request/board-access-request.model';
import { BoardAccessRequestService } from '../../services/board-access-request/board-access-request.service';
import { ManageBoardMemberService } from '../../services/manage-board-member/manage-board-member.service';

@Component({
  selector: 'app-board-member-request-accept',
  standalone: false,
  templateUrl: './board-member-request-accept.modal.html',
  styleUrl: './board-member-request-accept.modal.scss'
})
export class BoardMemberRequestAcceptModal {
  boardId!: string;
  accessRequests: BoardAccessRequestModel[] = [];

  constructor(
    private dialogRef: MatDialogRef<BoardMemberRequestAcceptModal>,
    private accessRequestsService: BoardAccessRequestService,
    @Inject(MAT_DIALOG_DATA) private data: { boardId: string, accessRequests: BoardAccessRequestModel[] }
  ) {
    this.boardId = data.boardId;
    this.accessRequests = data.accessRequests;
  }

  getAccessRequests() {
    this.accessRequestsService.getBoardAccessRequestByBoardId(this.boardId).subscribe(result => {
      if (result) {
        this.accessRequests = result;
      }
    });
  }

  resolveRequest(request: BoardAccessRequestModel, decision: boolean) {
      let resolve = {
        requestId: request.Id,
        decision: decision
      }

      this.accessRequestsService.resolveBoardAccessRequest(this.boardId, resolve).subscribe({
        next: (result) => {
          this.getAccessRequests();
        },
        error: (error) => {

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

  closeModal(): void {
    this.dialogRef.close(this.accessRequests);
  }
}
