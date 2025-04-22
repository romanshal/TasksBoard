import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardMembersModal } from './board-members.modal';

describe('BoardMembersComponent', () => {
  let component: BoardMembersModal;
  let fixture: ComponentFixture<BoardMembersModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BoardMembersModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardMembersModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
