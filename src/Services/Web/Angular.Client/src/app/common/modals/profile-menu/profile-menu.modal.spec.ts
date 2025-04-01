import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileMenuModal } from './profile-menu.modal';

describe('ProfileMenuModal', () => {
  let component: ProfileMenuModal;
  let fixture: ComponentFixture<ProfileMenuModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProfileMenuModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfileMenuModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
