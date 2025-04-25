import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardMemberPermissionsModal } from './board-member-permissions.modal';

describe('BoardMemberPermissionsModal', () => {
  let component: BoardMemberPermissionsModal;
  let fixture: ComponentFixture<BoardMemberPermissionsModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BoardMemberPermissionsModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardMemberPermissionsModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
