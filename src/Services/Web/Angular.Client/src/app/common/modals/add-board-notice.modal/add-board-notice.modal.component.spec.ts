import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddBoardNoticeModalComponent } from './add-board-notice.modal.component';

describe('AddBoardNoticeModalComponent', () => {
  let component: AddBoardNoticeModalComponent;
  let fixture: ComponentFixture<AddBoardNoticeModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddBoardNoticeModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddBoardNoticeModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
