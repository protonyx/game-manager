import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SimpleChange } from '@angular/core';

import { CurrentTurnComponent } from './current-turn.component';
import { Game, GameState, Player } from '../../models/models';

const makePlayer = (id: string, order: number = 1): Player => ({
  id,
  order,
  name: `Player ${id}`,
  state: 'Active',
  trackerValues: {},
});

const makeGame = (currentTurnPlayerId: string): Game => ({
  id: 'g1',
  name: 'Test Game',
  entryCode: 'ABC',
  state: GameState.InProgress,
  currentTurnPlayerId,
  trackers: [],
  options: { shareOtherPlayerTrackers: false },
  createdDate: new Date().toISOString(),
});

describe('CurrentTurnComponent', () => {
  let component: CurrentTurnComponent;
  let fixture: ComponentFixture<CurrentTurnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CurrentTurnComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(CurrentTurnComponent);
    component = fixture.componentInstance;
    component.game = null;
    component.players = null;
    component.currentPlayer = null;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('sets currentTurn to the player matching currentTurnPlayerId', () => {
    const p1 = makePlayer('p1', 1);
    const p2 = makePlayer('p2', 2);
    component.game = makeGame('p1');
    component.players = [p1, p2];
    component.currentPlayer = p2;
    component.ngOnChanges({ game: new SimpleChange(null, component.game, true) });
    expect(component.currentTurn?.id).toBe('p1');
  });

  it('sets nextTurn to the player after currentTurn', () => {
    const p1 = makePlayer('p1', 1);
    const p2 = makePlayer('p2', 2);
    component.game = makeGame('p1');
    component.players = [p1, p2];
    component.currentPlayer = p2;
    component.ngOnChanges({ game: new SimpleChange(null, component.game, true) });
    expect(component.nextTurn?.id).toBe('p2');
  });

  it('sets isMyTurn to true when currentPlayer is the current turn player', () => {
    const p1 = makePlayer('p1', 1);
    const p2 = makePlayer('p2', 2);
    component.game = makeGame('p1');
    component.players = [p1, p2];
    component.currentPlayer = p1;
    component.ngOnChanges({ game: new SimpleChange(null, component.game, true) });
    expect(component.isMyTurn).toBeTrue();
  });

  it('sets isMyTurn to false when currentPlayer is not the current turn player', () => {
    const p1 = makePlayer('p1', 1);
    const p2 = makePlayer('p2', 2);
    component.game = makeGame('p1');
    component.players = [p1, p2];
    component.currentPlayer = p2;
    component.ngOnChanges({ game: new SimpleChange(null, component.game, true) });
    expect(component.isMyTurn).toBeFalse();
  });

  it('emits endTurn when onEndTurn() is called', () => {
    spyOn(component.endTurn, 'emit');
    component.onEndTurn();
    expect(component.endTurn.emit).toHaveBeenCalled();
  });
});
