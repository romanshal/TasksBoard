import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardInfoModal } from './board-info.modal';

describe('BoardInfoComponent', () => {
  let component: BoardInfoModal;
  let fixture: ComponentFixture<BoardInfoModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BoardInfoModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardInfoModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
