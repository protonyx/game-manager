import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { provideMockStore } from '@ngrx/store/testing';

import { GameControlComponent } from './game-control.component';
import { GameState } from '../../models/models';

describe('GameControlComponent', () => {
  let component: GameControlComponent;
  let fixture: ComponentFixture<GameControlComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [GameControlComponent, NoopAnimationsModule],
      providers: [provideMockStore()],
    });
    fixture = TestBed.createComponent(GameControlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('stateLabel()', () => {
    it('returns "Lobby" for Preparing state', () => {
      expect(component.stateLabel(GameState.Preparing)).toBe('Lobby');
    });

    it('returns "In Progress" for InProgress state', () => {
      expect(component.stateLabel(GameState.InProgress)).toBe('In Progress');
    });

    it('returns "Complete" for Complete state', () => {
      expect(component.stateLabel(GameState.Complete)).toBe('Complete');
    });

    it('returns empty string for null', () => {
      expect(component.stateLabel(null)).toBe('');
    });
  });

  describe('event emitters', () => {
    it('emits startGame when onStartGame() is called', () => {
      spyOn(component.startGame, 'emit');
      component.onStartGame();
      expect(component.startGame.emit).toHaveBeenCalled();
    });

    it('emits advanceTurn when onAdvanceTurn() is called', () => {
      spyOn(component.advanceTurn, 'emit');
      component.onAdvanceTurn();
      expect(component.advanceTurn.emit).toHaveBeenCalled();
    });

    it('emits endGame when onEndGame() is called', () => {
      spyOn(component.endGame, 'emit');
      component.onEndGame();
      expect(component.endGame.emit).toHaveBeenCalled();
    });
  });
});
