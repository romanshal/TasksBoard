import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationMenuModal } from './notification-menu.modal';

describe('NotificationMenuModal', () => {
  let component: NotificationMenuModal;
  let fixture: ComponentFixture<NotificationMenuModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [NotificationMenuModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotificationMenuModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
