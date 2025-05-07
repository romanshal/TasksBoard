import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardMemberRequestAcceptModal } from './board-member-request-accept.modal';

describe('BoardMemberRequestAcceptModal', () => {
  let component: BoardMemberRequestAcceptModal;
  let fixture: ComponentFixture<BoardMemberRequestAcceptModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BoardMemberRequestAcceptModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardMemberRequestAcceptModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
