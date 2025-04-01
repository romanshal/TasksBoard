import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileMenuModalComponent } from './profile-menu.modal.component';

describe('ProfileMenuModalComponent', () => {
  let component: ProfileMenuModalComponent;
  let fixture: ComponentFixture<ProfileMenuModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProfileMenuModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfileMenuModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
