import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaginatorNewComponent } from './paginator-new.component';

describe('PaginatorNewComponent', () => {
  let component: PaginatorNewComponent;
  let fixture: ComponentFixture<PaginatorNewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PaginatorNewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaginatorNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
