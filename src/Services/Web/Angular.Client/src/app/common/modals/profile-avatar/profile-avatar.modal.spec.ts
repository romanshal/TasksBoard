import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileAvatarModal } from './profile-avatar.modal';

describe('ProfileAvatarModal', () => {
  let component: ProfileAvatarModal;
  let fixture: ComponentFixture<ProfileAvatarModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProfileAvatarModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfileAvatarModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
