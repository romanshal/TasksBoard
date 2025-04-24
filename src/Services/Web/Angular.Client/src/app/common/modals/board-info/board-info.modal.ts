import { AfterViewInit, Component, ElementRef, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { BoardModel } from '../../models/board/board.model';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BoardMemberModel } from '../../models/board-member/board-member.model';
import { BoardPermission } from '../../models/board-permission/board-permission.model';
import { ManageBoardService } from '../../services/manage-board/manage-board.service';

@Component({
  selector: 'app-board-info',
  standalone: false,
  templateUrl: './board-info.modal.html',
  styleUrl: './board-info.modal.scss'
})
export class BoardInfoModal implements OnInit, AfterViewInit {
  board!: BoardModel;
  form!: FormGroup;

  userId!: string;

  isOwner = false;

  public disabledActions = true;

  public members: BoardMemberModel[] = [];
  permissions: BoardPermission[] = [];

  inputWidth: number = 0;

  @ViewChild('textMeasure', { static: false }) textMeasure!: ElementRef<HTMLSpanElement>;

  newTag: string = '';

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<BoardInfoModal>,
    private manageBoardService: ManageBoardService,
    @Inject(MAT_DIALOG_DATA) private data: { board: BoardModel, members: BoardMemberModel[], permissions: BoardPermission[], userId: string, isOwner: boolean }
  ) {
    this.board = data.board;
    this.isOwner = data.isOwner;
    this.members = data.members;
    this.permissions = data.permissions;
    this.userId = data.userId;
  }

  ngOnInit(): void {
    this.initForm();
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.updateWidth();
    }, 0);
  }

  initForm() {
    this.form = this.fb.group({
      name: [{ value: this.board.Name, disabled: this.disabledActions }, Validators.required],
      description: [{ value: this.board.Description, disabled: this.disabledActions }, Validators.required],
      tags: this.fb.array(this.board.Tags.map(t => this.fb.control(t)))
    });
  }

  get tags(): FormArray {
    return this.form.get('tags') as FormArray;
  }

  addTag() {
    if (this.newTag) {
      this.newTag = this.newTag.trim();

      this.newTag = this.newTag.replaceAll('#', '');

      this.newTag = '#' + this.newTag;

      const exists = this.tags.controls.some(control => control.value === this.newTag);
      if (exists) {
        this.newTag = '';
        return;
      }

      this.tags.push(this.fb.control(this.newTag));

      this.newTag = '';
    }
  }

  deleteTag(index: number) {
    this.tags.removeAt(index);
  }

  updateWidth() {
    const measuredText = this.form.controls['name']?.value;
    // Записываем значение в скрытый span для замера
    this.textMeasure.nativeElement.textContent = measuredText;
    // Получаем ширину span и задаём её в input
    this.inputWidth = this.textMeasure.nativeElement.offsetWidth + 5;
  }

  showActions() {
    this.disabledActions = false;
    this.form.controls['name'].enable();
    this.form.controls['description'].enable();
  }

  submitForm() {
    if (this.form.valid) {
      this.manageBoardService.updateBoard(this.board.Id, this.form.value).subscribe(result => {
        this.closeModal('updated')
      });
    } else {
      console.error('Форма недействительна!');
    }
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
