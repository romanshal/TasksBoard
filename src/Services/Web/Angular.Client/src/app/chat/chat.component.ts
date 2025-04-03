import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { MessageModel } from '../common/models/message/message.model';
import { SessionStorageService } from '../common/services/session-storage/session-storage.service';

@Component({
  selector: 'app-chat',
  standalone: false,
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent implements AfterViewInit  {
  public userId: any;

  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  messages: MessageModel[] = [
    new MessageModel('userId', 'Test2', new Date(), 'message'),
    new MessageModel('019589fd-5bf2-7dd0-a5dc-d30fb83dbc97', 'Test', new Date(), 'mess agemessag emessage messagemes sagemessa gemessag emessagemes sageme ssagemessa gemes sa geme ssage mess agemessagemessa gemessagemessagemessage'),
    new MessageModel('userId', 'Test2', new Date(), 'mess agemessag emessage messagemes sagemessa gemessag emessagemes sageme ssagemessa gemes sa geme ssage mess agemessagemessa gemessagemessagemessage message messagemessage'),
    new MessageModel('019589fd-5bf2-7dd0-a5dc-d30fb83dbc97', 'Test', new Date(), 'mess agemessag emessage messagemes sagemessa gemessag emessagemes sageme ssageme'),
  ]

  constructor(
    private sessionStorageService: SessionStorageService,
  ) {
    this.userId = this.sessionStorageService.getItem(this.sessionStorageService.userIdKey);
  }

  ngAfterViewInit(): void {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    const container = this.messagesContainer.nativeElement;
    container.scroll({
      top: container.scrollHeight,
      behavior: 'smooth'
    });
  }

  close() {


  }
}
