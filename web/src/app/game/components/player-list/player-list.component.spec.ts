import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SimpleChange } from '@angular/core';

import { PlayerListComponent } from './player-list.component';
import { Game, GameState, Player, Tracker } from '../../models/models';

const makePlayer = (id: string, trackerValues: Record<string, number> = {}): Player => ({
  id,
  order: 1,
  name: `Player ${id}`,
  color: '#D32F2F',
  state: 'Active',
  trackerValues,
});

const makeGame = (trackers: Tracker[] = [], currentTurnPlayerId?: string): Game => ({
  id: 'g1',
  name: 'Test Game',
  entryCode: 'ABC',
  state: GameState.InProgress,
  currentTurnPlayerId,
  trackers,
  options: { shareOtherPlayerTrackers: false },
  createdDate: new Date().toISOString(),
});

describe('PlayerListComponent', () => {
  let component: PlayerListComponent;
  let fixture: ComponentFixture<PlayerListComponent>;

  const triggerChanges = () => {
    component.ngOnChanges({ game: new SimpleChange(null, component.game, true) });
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlayerListComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(PlayerListComponent);
    component = fixture.componentInstance;
    component.game = null;
    component.players = null;
    component.currentPlayer = null;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnChanges() columnsToDisplay', () => {
    it('does not include "actions" when isHost is false', () => {
      component.game = makeGame();
      component.players = [makePlayer('p1')];
      component.isHost = false;
      triggerChanges();
      expect(component.columnsToDisplay).not.toContain('actions');
    });

    it('includes "actions" as the first column when isHost is true', () => {
      component.game = makeGame();
      component.players = [makePlayer('p1')];
      component.isHost = true;
      triggerChanges();
      expect(component.columnsToDisplay[0]).toBe('actions');
    });

    it('includes tracker ids in columnsToDisplay', () => {
      const trackers: Tracker[] = [
        { id: 'tracker-hp', name: 'HP', startingValue: 20 },
        { id: 'tracker-mp', name: 'MP', startingValue: 10 },
      ];
      component.game = makeGame(trackers);
      component.players = [makePlayer('p1')];
      component.isHost = false;
      triggerChanges();
      expect(component.columnsToDisplay).toContain('tracker-hp');
      expect(component.columnsToDisplay).toContain('tracker-mp');
    });
  });

  describe('checkIsMe()', () => {
    it('returns true when player.id matches currentPlayer.id', () => {
      component.currentPlayer = makePlayer('p1');
      expect(component.checkIsMe(makePlayer('p1'))).toBeTrue();
    });

    it('returns false when currentPlayer is null', () => {
      component.currentPlayer = null;
      expect(component.checkIsMe(makePlayer('p1'))).toBeFalse();
    });
  });

  describe('getTrackerValue()', () => {
    it('returns the tracker value as a string when it exists', () => {
      const player = makePlayer('p1', { t1: 42 });
      expect(component.getTrackerValue(player, 't1')).toBe('42');
    });

    it('returns "???" when trackerId is not in trackerValues', () => {
      const player = makePlayer('p1', {});
      expect(component.getTrackerValue(player, 'missing')).toBe('???');
    });
  });

  describe('checkIsPlayerTurn()', () => {
    it('returns true when player is the current turn player', () => {
      component.game = makeGame([], 'p1');
      expect(component.checkIsPlayerTurn(makePlayer('p1'))).toBeTrue();
    });

    it('returns false when player is not the current turn player', () => {
      component.game = makeGame([], 'p2');
      expect(component.checkIsPlayerTurn(makePlayer('p1'))).toBeFalse();
    });
  });

  describe('event emitters', () => {
    it('emits editPlayer when handleEditPlayer() is called', () => {
      spyOn(component.editPlayer, 'emit');
      const player = makePlayer('p1');
      component.handleEditPlayer(player);
      expect(component.editPlayer.emit).toHaveBeenCalledWith(player);
    });

    it('emits kickPlayer when handleKickPlayer() is called', () => {
      spyOn(component.kickPlayer, 'emit');
      const player = makePlayer('p1');
      component.handleKickPlayer(player);
      expect(component.kickPlayer.emit).toHaveBeenCalledWith(player);
    });
  });

  describe('compactMode', () => {
    it('defaults to false', () => {
      expect(component.compactMode).toBeFalse();
    });

    it('when true, search field is not rendered even with >10 players', () => {
      component.compactMode = true;
      component.game = makeGame();
      // Create 11 players to normally trigger the search field
      component.players = Array.from({length: 11}, (_, i) => makePlayer(`p${i}`));
      component.ngOnChanges({ game: new SimpleChange(null, component.game, true) });
      fixture.detectChanges();
      const searchField = fixture.nativeElement.querySelector('.search-field');
      expect(searchField).toBeNull();
    });

    it('when false, renders normally', () => {
      component.compactMode = false;
      expect(component.compactMode).toBeFalse();
    });
  });
});
