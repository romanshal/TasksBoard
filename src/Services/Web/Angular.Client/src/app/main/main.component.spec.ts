import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MainComponent } from './main.component';

describe('HomeComponent', () => {
  let component: MainComponent;
  let fixture: ComponentFixture<MainComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MainComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  //it('should have title as "Добро пожаловать на начальную страницу!"', () => {
  //  expect(component.title).toBe('Добро пожаловать на начальную страницу!');
  //});

  //it('should have description as "Это место, где начинается ваше путешествие!"', () => {
  //  expect(component.description).toBe('Это место, где начинается ваше путешествие!');
  //});
});
