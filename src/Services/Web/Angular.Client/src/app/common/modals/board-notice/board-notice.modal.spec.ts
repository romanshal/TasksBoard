import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardNoticeModal } from './board-notice.modal';

describe('BoardNoticeModal', () => {
  let component: BoardNoticeModal;
  let fixture: ComponentFixture<BoardNoticeModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BoardNoticeModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardNoticeModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
