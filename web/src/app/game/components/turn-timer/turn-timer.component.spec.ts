import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { TurnTimerComponent } from './turn-timer.component';

describe('TurnTimerComponent', () => {
  let component: TurnTimerComponent;
  let fixture: ComponentFixture<TurnTimerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TurnTimerComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(TurnTimerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('returns zeros when startTime is null', () => {
    component.startTime = null;
    const result = component.getElapsedTime();
    expect(result).toEqual({ hours: 0, minutes: 0, seconds: 0 });
  });

  it('returns zeros when startTime is undefined', () => {
    component.startTime = undefined;
    const result = component.getElapsedTime();
    expect(result).toEqual({ hours: 0, minutes: 0, seconds: 0 });
  });

  it('returns correct seconds for a time 30 seconds ago', () => {
    component.startTime = new Date(Date.now() - 30 * 1000).toISOString();
    const result = component.getElapsedTime();
    expect(result.hours).toBe(0);
    expect(result.minutes).toBe(0);
    expect(result.seconds).toBeGreaterThanOrEqual(30);
    expect(result.seconds).toBeLessThan(32);
  });

  it('returns correct minutes and seconds for a time 90 seconds ago', () => {
    component.startTime = new Date(Date.now() - 90 * 1000).toISOString();
    const result = component.getElapsedTime();
    expect(result.hours).toBe(0);
    expect(result.minutes).toBe(1);
    expect(result.seconds).toBeGreaterThanOrEqual(30);
    expect(result.seconds).toBeLessThan(32);
  });

  it('returns correct hours, minutes, and seconds for a time 3725 seconds ago', () => {
    component.startTime = new Date(Date.now() - 3725 * 1000).toISOString();
    const result = component.getElapsedTime();
    expect(result.hours).toBe(1);
    expect(result.minutes).toBe(2);
    expect(result.seconds).toBeGreaterThanOrEqual(5);
    expect(result.seconds).toBeLessThan(7);
  });
});
