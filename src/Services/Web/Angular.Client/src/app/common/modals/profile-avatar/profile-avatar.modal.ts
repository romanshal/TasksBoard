import { Component, ElementRef, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UserService } from '../../services/user/user.service';

interface DefaultAvatar {
  src: string,
  selected: boolean
}

@Component({
  selector: 'app-profile-avatar',
  standalone: false,
  templateUrl: './profile-avatar.modal.html',
  styleUrl: './profile-avatar.modal.scss'
})
export class ProfileAvatarModal implements OnInit {
  userId!: string;

  selectedAvatar: string = '';
  selectedFile: File | null = null;
  selectedAvatarSrc: string = '';

  avatars: string[] = [
    'avatar1.png',
    'avatar2.png',
    'avatar3.png',
    'avatar4.png',
    'avatar5.png',
    'avatar6.png',
    'avatar7.png',
    'avatar8.png',
    'avatar9.png',
    'avatar10.png',
    'avatar11.png',
    'avatar12.png',
    'avatar13.png',
    'avatar14.png',
    'avatar15.png',
    'avatar16.png',
    'avatar17.png',
    'avatar18.png',
    'avatar19.png',
    'avatar20.png',
    'avatar21.png',
    'avatar22.png',
    'avatar23.png',
    'avatar24.png',
    'avatar25.png',
    'avatar26.png',
    'avatar27.png',
  ];

  private allowedTypes = ['image/jpeg', 'image/png', 'image/jpg'];

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  constructor(
    private dialogRef: MatDialogRef<ProfileAvatarModal>,
    private userService: UserService,
    @Inject(MAT_DIALOG_DATA) private data: { userId: string, avatar: string }
  ) {
    this.userId = data.userId;

    if (data.avatar) {
      this.selectedAvatarSrc = data.avatar;
    }
  }

  ngOnInit(): void {

  }

  selectDefaultAvatar(image: string) {
    this.selectedAvatar = image;
    this.selectedAvatarSrc = image;
    this.selectedFile = null;
  }

  triggerFileInput(): void {
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {

      const file = input.files[0];
      if (!this.allowedTypes.includes(file.type)) {
        input.value = '';

        return;
      }

      this.selectedFile = file;

      const reader = new FileReader();
      reader.onload = () => {
        this.selectedAvatarSrc = reader.result as string;
      };
      reader.readAsDataURL(this.selectedFile);

      this.selectedAvatar = '';
    }
  }

  updateUserAvatar() {
    if (!this.selectedFile && !this.selectedAvatar) {
      return;
    }

    if (!this.selectedFile && this.selectedAvatar) {
      fetch(this.selectedAvatar)
        .then(res => res.blob())
        .then(blob => {
          const fileName = this.selectedAvatar;
          this.selectedFile = new File([blob], fileName, { type: blob.type });

          this.uploadImage();
        })
        .catch(error => {
          console.error('Ошибка при загрузке картинки из массива', error);
        });
    } else if (this.selectedFile) {
      this.uploadImage();
    }
  }

  uploadImage() {
    const formData = new FormData();
    formData.append('image', this.selectedFile as File);

    this.userService.updateUserAvatar(this.userId, formData).subscribe({
      next: () => {
        this.closeModal(this.selectedAvatarSrc);
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

  closeModal(result?: any): void {
    this.dialogRef.close(result);
  }
}
