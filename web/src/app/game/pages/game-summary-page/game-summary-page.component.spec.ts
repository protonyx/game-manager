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
});
