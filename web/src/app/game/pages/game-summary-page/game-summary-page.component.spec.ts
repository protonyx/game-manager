import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';

import { GameSummaryPageComponent } from './game-summary-page.component';

describe('GameSummaryPageComponent', () => {
  let component: GameSummaryPageComponent;
  let fixture: ComponentFixture<GameSummaryPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GameSummaryPageComponent],
      providers: [provideMockStore()],
    });
    fixture = TestBed.createComponent(GameSummaryPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('formatTurnDuration', () => {
    it('formats seconds under a minute', () => {
      expect(component.formatTurnDuration(30)).toBe('0:0:30');
    });

    it('formats minutes and seconds', () => {
      expect(component.formatTurnDuration(65)).toBe('0:1:5');
    });

    it('formats hours, minutes, and seconds', () => {
      expect(component.formatTurnDuration(3661)).toBe('1:1:1');
    });
  });
});
