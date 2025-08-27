import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { BoardMessageModel } from '../common/models/board-message/board-message.model';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { BoardMessageService } from '../common/services/board-message/board-message.service';
import { ChatService } from '../common/services/chat/chat.service';
import { BoardMemberModel } from '../common/models/board-member/board-member.model';
import { Subscription } from 'rxjs';
import { UserService } from '../common/services/user/user.service';
import { AuthStateService } from '../common/services/auth-state/auth-state.service';

@Component({
  selector: 'app-chat',
  standalone: false,
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent implements OnInit {
  public userId: any;

  isEmojiOpen = false;

  @Input({ required: true }) public boardId!: string;
  @Input({ required: true }) public currentMember!: BoardMemberModel;

  pageIndex = 1;
  pageSize = 10;

  messages: BoardMessageModel[] = [];

  private subs: Subscription[] = [];
  state: 'disconnected' | 'connecting' | 'connected' = 'disconnected';

  messageForUpdate?: BoardMessageModel;

  isChatOpen = false;

  message: string = '';

  unreadCount = 0;
  unread: string = '';

  openedContentActionsMenuId = '';

  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  constructor(
    private authStateService: AuthStateService,
    private boardMessageService: BoardMessageService,
    private chatService: ChatService
  ) {
    this.authStateService.currentUser$.subscribe(user => {
      this.userId = user?.Id;
    });
  }

  async ngOnInit(): Promise<void> {
    this.boardMessageService.getMessages(this.boardId, this.pageIndex, this.pageSize).subscribe(result => {
      if (result) {
        this.messages = result.reverse();
      }
    });

    this.subs.push(
      this.chatService.onMessage().subscribe(m => {
        this.messages.push(m);
        this.scrollToBottom();

        if (!this.isChatOpen) {
          this.unreadCount++;

          if (this.unreadCount > 100) {
            this.unread = '!';
          } else {
            this.unread = this.unreadCount.toString();
          }
        }
      }),
      this.chatService.onEdit().subscribe(msg => {
        const idx = this.messages.findIndex(m => m.Id === msg.Id);
        if (idx === -1) return;

        const edited = {
          ...this.messages[idx],
          ...msg
        };

        // Обновляем элемент
        this.messages.splice(idx, 1, edited);

        // Если компонент с OnPush — обновим ссылку на массив
        this.messages = [...this.messages];
      }),
      this.chatService.onDelete().subscribe(id => {
        const idx = this.messages.findIndex(m => m.Id === id);
        if (idx === -1) return;

        this.messages[idx].IsDeleted = true;
      }),
      this.chatService.connectionState().subscribe(s => (this.state = s))
    );

    await this.chatService.startConnection();
    if (this.boardId) {
      await this.chatService.switchBoard(this.boardId, this.userId);
    }
  }

  changeChatVisability() {
    if (!this.isChatOpen) {
      this.unreadCount = 0;
      this.unread = '';
    }

    this.isEmojiOpen = false;
    this.isChatOpen = !this.isChatOpen;
    this.scrollToBottom();
  }

  scrollToBottom() {
    if (this.isChatOpen) {
      setTimeout(() => {
        const container = document.getElementById('messagesContainer')!;
        container.scroll({
          top: container.scrollHeight,
          behavior: 'auto'
        });
      }, 0);
    }
  }

  sendMessage() {
    if (!this.message) {
      return;
    }

    this.message = this.message.trim();

    if (this.messageForUpdate) {
      // edit message
      let editMessage = {
        boardMessageId: this.messageForUpdate.Id,
        message: this.message
      };

      this.boardMessageService.editMessage(this.boardId, editMessage).subscribe(result => {
        if (result) {
          let mes = this.messages.find(message => message.Id === this.messageForUpdate?.Id)!;
          mes.Message = this.message;
          mes.ModifiedAt = new Date();

          this.isEmojiOpen = false;
          this.removeMessageForUpdate();
        }
      })
    } else {
      // new message
      let message = {
        memberId: this.currentMember.Id,
        accountId: this.currentMember.AccountId,
        message: this.message
      };

      this.boardMessageService.sendMessage(this.boardId, message).subscribe(result => {
        if (result) {
          this.message = '';
          this.isEmojiOpen = false;
        }
      })
    }
  }

  showMoreMessages() {
    // Если сообщений еще нет, ничего менять не нужно
    if (this.messages.length === 0) {
      return;
    }

    this.pageIndex++;

    const container = this.messagesContainer.nativeElement;
    const previousScrollHeight = container.scrollHeight;

    this.boardMessageService.getMessages(this.boardId, this.pageIndex, this.pageSize).subscribe(result => {
      if (result) {
        this.messages.unshift(...result.reverse());

        setTimeout(() => {
          const newScrollHeight = container.scrollHeight;
          container.scrollTop = previousScrollHeight - newScrollHeight;
        });
      }
    });
  }

  showContentActionsMenu(messageId: string) {
    if (this.openedContentActionsMenuId === messageId) {
      this.openedContentActionsMenuId = '';
    } else {
      this.openedContentActionsMenuId = messageId;
    }
  }

  showEmoji() {
    this.isEmojiOpen = !this.isEmojiOpen;
    this.scrollToBottom();
  }

  addEmoji(event: any) {
    this.message += event.emoji.native;
  }

  editMessage(message: BoardMessageModel) {
    this.messageForUpdate = message;
    this.message = message.Message;
    this.openedContentActionsMenuId = '';
  }

  removeMessageForUpdate() {
    this.messageForUpdate = undefined;
    this.message = '';
  }

  deleteMessage(message: BoardMessageModel) {
    this.boardMessageService.deleteMessage(this.boardId, message.Id).subscribe(result => {
      if (result) {
        this.openedContentActionsMenuId = '';
        message.IsDeleted = true;
      }
    });
  }
}
