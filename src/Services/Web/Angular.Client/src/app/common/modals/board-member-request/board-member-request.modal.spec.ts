import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardMemberRequestModal } from './board-member-request.modal';

describe('BoardMemberRequestModal', () => {
  let component: BoardMemberRequestModal;
  let fixture: ComponentFixture<BoardMemberRequestModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BoardMemberRequestModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardMemberRequestModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
