import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationResultMessageModal } from './operation-result-message.modal';

describe('OperationResultMessageModal', () => {
  let component: OperationResultMessageModal;
  let fixture: ComponentFixture<OperationResultMessageModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OperationResultMessageModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OperationResultMessageModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
