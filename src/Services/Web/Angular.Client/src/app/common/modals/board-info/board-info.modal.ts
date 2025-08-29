import { AfterViewInit, Component, ElementRef, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { BoardModel } from '../../models/board/board.model';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ManageBoardService } from '../../services/manage-board/manage-board.service';
import { UserInfoModel } from '../../models/user/user-info.model';
import { BoardService } from '../../services/board/board.service';
import { DeleteConfirmationModal } from '../delete-confirmation/delete-confirmation.modal';
import { Router } from '@angular/router';
import { AuthSessionService } from '../../services/auth-session/auth-session.service';

@Component({
  selector: 'app-board-info',
  standalone: false,
  templateUrl: './board-info.modal.html',
  styleUrl: './board-info.modal.scss'
})
export class BoardInfoModal implements OnInit, AfterViewInit {
  board?: BoardModel;
  form!: FormGroup;
  isOwner = false;

  private currentUser?: UserInfoModel | null;

  public disabledActions = true;

  inputWidth: number = 0;

  @ViewChild('textMeasure', { static: false }) textMeasure!: ElementRef<HTMLSpanElement>;

  newTag: string = '';

  newBoard = false;
  formSubmitted = false;

  selectedFile: File | null = null;
  previewUrl: string | ArrayBuffer | null | undefined = null;
  dragOver: boolean = false;
  private allowedTypes = ['image/jpeg', 'image/png', 'image/jpg'];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<BoardInfoModal>,
    private boardService: BoardService,
    private manageBoardService: ManageBoardService,
    private authSessionService: AuthSessionService,
    private router: Router,
    @Inject(MAT_DIALOG_DATA) private data: { board: BoardModel, isOwner: boolean }
  ) {
    if (data.board === undefined) {
      this.newBoard = true;
      this.disabledActions = false;

      this.authSessionService.currentUser$.subscribe(user => {
        this.currentUser = user;
      });
    }
    else {
      this.board = data.board;
    }

    this.isOwner = data.isOwner;
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
      ownerId: [this.currentUser?.Id],
      ownerNickname: [this.currentUser?.Username],
      name: [{ value: this.board?.Name ?? '', disabled: this.disabledActions }, Validators.required],
      description: [{ value: this.board?.Description ?? '', disabled: this.disabledActions }, Validators.required],
      tags: this.fb.array(this.board?.Tags?.map(t => this.fb.control(t)) ?? []),
      public: [{ value: this.board?.Public ?? false, disabled: this.disabledActions }, Validators.required],
      image: []
    });

    if (this.newBoard || !this.board!.Image) {
      return;
    }

    const mimeTypeMap: Record<string, string> = {
      '.png': 'image/png',
      '.jpg': 'image/jpeg',
      '.jpeg': 'image/jpeg',
      '.gif': 'image/gif'
    };

    const mimeType = mimeTypeMap[this.board!.ImageExtension!] || 'application/octet-stream';
    let base64Image = this.board!.Image;

    if (!base64Image.startsWith('data:')) {
      base64Image = `data:${mimeType};base64,${base64Image}`;
    }

    this.previewUrl = base64Image;
    const file = this.base64ToFile(base64Image, `loaded_image${this.board!.ImageExtension}`, mimeType);
    this.form.patchValue({ image: file });

  }

  base64ToFile(dataUrl: string, filename: string, mimeType: string): File {
    const arr = dataUrl.split(',');
    const bstr = atob(arr[1]);
    const n = bstr.length;
    const u8arr = new Uint8Array(n);
    for (let i = 0; i < n; i++) {
      u8arr[i] = bstr.charCodeAt(i);
    }
    return new File([u8arr], filename, { type: mimeType });
  }

  get tags(): FormArray {
    return this.form.get('tags') as FormArray;
  }

  addTag() {
    if (this.newTag) {
      this.newTag = this.newTag.trim().replaceAll('#', '').replaceAll(' ', '');

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
    this.form.controls['public'].enable();
  }

  onFileChange(event: Event): void {
    if (this.disabledActions) {
      return;
    }

    const input = event.target as HTMLInputElement;

    if (input.files && input.files?.length) {
      this.SetFile(input.files[0]);
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();

    if (this.disabledActions) {
      return;
    }

    this.dragOver = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();

    if (this.disabledActions) {
      return;
    }

    this.dragOver = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();

    if (this.disabledActions) {
      return;
    }

    this.dragOver = false;

    if (event.dataTransfer && event.dataTransfer.files && event.dataTransfer.files.length) {
      let file = event.dataTransfer.files[0]

      this.SetFile(file);

      event.dataTransfer.clearData();
    }
  }

  private SetFile(file: File) {
    if (!this.allowedTypes.includes(file.type)) {
      // Здесь можно добавить уведомление об ошибке, если файл не соответствует
      return;
    }

    this.selectedFile = file;

    this.form.patchValue({ image: file });
    this.form.get('image')?.updateValueAndValidity();

    const reader = new FileReader();

    reader.onload = (e: ProgressEvent<FileReader>) => {
      this.previewUrl = e.target?.result;
    };

    reader.readAsDataURL(file);
  }

  deletePreviewImage() {
    this.selectedFile = null;
    this.previewUrl = null;
    this.form.controls['image'].setValue(null);
  }

  submitForm() {
    this.formSubmitted = true;

    if (this.form.invalid) {
      console.error('Форма недействительна!');
      return;
    }

    if (this.newBoard) {
      this.createBoard();
    } else {
      this.updateBoard();
    }
  }

  createBoard() {
    const formData = new FormData();
    formData.append('name', this.form.get('name')?.value);
    formData.append('description', this.form.get('description')?.value);
    formData.append('public', this.form.get('public')?.value)
    formData.append('ownerId', this.form.get('ownerId')?.value)
    formData.append('ownerNickname', this.form.get('ownerNickname')?.value)

    const tagsArray = this.form.get('tags')?.value;
    if (Array.isArray(tagsArray)) {
      tagsArray.forEach(tag => formData.append('tags[]', tag));
    }

    const imageFile = this.form.get('image')?.value;
    if (imageFile) {
      formData.append('image', imageFile);
    }

    this.boardService.createBoard(formData).subscribe(result => {
      if (result) {
        this.closeModal(result);
      }
    });
  }

  updateBoard() {
    const formData = new FormData();
    formData.append('name', this.form.get('name')?.value);
    formData.append('description', this.form.get('description')?.value);
    formData.append('public', this.form.get('public')?.value)

    const tagsArray = this.form.get('tags')?.value;
    if (Array.isArray(tagsArray)) {
      tagsArray.forEach(tag => formData.append('tags[]', tag));
    }

    const imageFile = this.form.get('image')?.value;
    if (imageFile) {
      formData.append('image', imageFile);
    }

    this.manageBoardService.updateBoard(this.board!.Id, formData).subscribe(result => {
      this.closeModal('updated')
    });
  }

  openDeleteConfirmation() {
    if (this.newBoard) {
      return;
    }

    this.dialog.open(DeleteConfirmationModal, {
      data: {
        element: 'board',
        elementName: this.board?.Name,
        secondConfirme: true
      }
    }).afterClosed().subscribe(result => {
      if (result === 'confirmed') {
        this.manageBoardService.deleteBoard(this.board!.Id).subscribe(result => {
          if (result) {
            this.closeModal();
            this.router.navigate(['/boards']);
          }
        })
      }
    })
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
