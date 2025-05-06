import { AfterViewChecked, ChangeDetectionStrategy, ChangeDetectorRef, Component, ElementRef, input, Input, OnInit, ViewChild } from '@angular/core';
import { BoardMessageModel } from '../common/models/board-message/board-message.model';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';
import { BoardMessageService } from '../common/services/board-message/board-message.service';
import { ChatService } from '../common/services/chat/chat.service';
import { BoardMemberModel } from '../common/models/board-member/board-member.model';

@Component({
  selector: 'app-chat',
  standalone: false,
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent implements OnInit {
  public userId: any;

  @Input({ required: true }) public boardId!: string;
  @Input({ required: true }) public currentMember!: BoardMemberModel;

  pageIndex = 1;
  pageSize = 10;

  messages: BoardMessageModel[] = [];

  isChatOpen = false;

  message: string = '';

  unreadCount = 0;
  unread: string = '';

  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  constructor(
    private sessionStorageService: SessionStorageService,
    private boardMessageService: BoardMessageService,
    private chatService: ChatService
  ) {
    this.userId = this.sessionStorageService.getItem(this.sessionStorageService.userIdKey);
  }

  ngOnInit(): void {
    this.boardMessageService.getMessages(this.boardId, this.pageIndex, this.pageSize).subscribe(result => {
      if (result) {
        this.messages = result.reverse();
      }
    });

    this.chatService.messages$.subscribe(msg => {
      if (msg) {
        this.messages.push(msg);
        this.scrollToBottom();

        if (!this.isChatOpen) {
          this.unreadCount++;

          if (this.unreadCount > 100) {
            this.unread = '!';
          } else {
            this.unread = this.unreadCount.toString();
          }
        }
      }
    });
  }

  changeChatVisability() {
    if (!this.isChatOpen) {
      this.unreadCount = 0;
      this.unread = '';
    }

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
    if (this.message) {
      this.message = this.message.trim();

      let message = {
        memberId: this.currentMember.Id,
        accountId: this.currentMember.AccountId,
        memberNickname: this.currentMember.Nickname,
        message: this.message
      };

      this.boardMessageService.sendMessage(this.boardId, message).subscribe(result => {
        if (result) {
          this.message = '';
        }
      })
    }
  }

  showMoreMessages() {
    this.pageIndex++;
    
    const container = this.messagesContainer.nativeElement;

    // Если сообщений еще нет, ничего менять не нужно
    if (this.messages.length === 0) {
      return;
    }

    // Берем id первого сообщения из массива
    const firstMessageId = this.messages[0].Id;

    this.boardMessageService.getMessages(this.boardId, this.pageIndex, this.pageSize).subscribe(result => {
      if (result) {
        this.messages.unshift(...result.reverse());

        setTimeout(() => {
          const firstMessageElement: HTMLElement | null = document.getElementById(`${firstMessageId}`);

          container.scroll({
            top: firstMessageElement!.scrollHeight - 50,
            behavior: 'auto'
          });
        });
      }
    });
  }
}
