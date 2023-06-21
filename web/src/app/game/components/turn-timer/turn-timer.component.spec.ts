import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TurnTimerComponent } from './turn-timer.component';

describe('TurnTimerComponent', () => {
  let component: TurnTimerComponent;
  let fixture: ComponentFixture<TurnTimerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [TurnTimerComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(TurnTimerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
