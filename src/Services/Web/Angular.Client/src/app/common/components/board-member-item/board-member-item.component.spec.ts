import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardMemberItemComponent } from './board-member-item.component';

describe('BoardMemberItemComponent', () => {
  let component: BoardMemberItemComponent;
  let fixture: ComponentFixture<BoardMemberItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BoardMemberItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardMemberItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
