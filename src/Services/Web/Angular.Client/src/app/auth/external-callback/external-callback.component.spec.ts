import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExternalCallbackComponent } from './external-callback.component';

describe('ExternalCallbackComponent', () => {
  let component: ExternalCallbackComponent;
  let fixture: ComponentFixture<ExternalCallbackComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ExternalCallbackComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExternalCallbackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
