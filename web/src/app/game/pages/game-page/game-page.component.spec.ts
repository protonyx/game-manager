import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { GamePageComponent } from './game-page.component';
import { GameActions } from '../../state/game.actions';
import { Game, Player } from '../../models/models';

const initialState = {
  game: {
    game: {
      id: '1',
      entryCode: '1234',
      currentTurnPlayerId: '1',
      status: 'Started',
      trackers: []
    },
    players: {
      ids: ['1'],
      entities: {
        '1': { id: '1', name: 'Player 1', order: 1 }
      }
    },
    trackers: []
  }
};

const mockGame = { id: 'game-1' } as Game;
const mockPlayer: Player = { id: 'player-1', name: 'Alice', order: 1, state: 'Active', trackerValues: {} };

describe('GamePageComponent', () => {
  let component: GamePageComponent;
  let fixture: ComponentFixture<GamePageComponent>;
  let mockStore: MockStore;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GamePageComponent, NoopAnimationsModule],
      providers: [provideMockStore({ initialState })],
    }).compileComponents();

    mockStore = TestBed.inject(MockStore);
    fixture = TestBed.createComponent(GamePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('onEndTurn dispatches endTurn action', () => {
    spyOn(mockStore, 'dispatch');
    component.onEndTurn(mockGame);
    expect(mockStore.dispatch).toHaveBeenCalledWith(
      GameActions.endTurn({ gameId: mockGame.id })
    );
  });

  it('onStartGame dispatches startGame action', () => {
    spyOn(mockStore, 'dispatch');
    component.onStartGame(mockGame);
    expect(mockStore.dispatch).toHaveBeenCalledWith(
      GameActions.startGame({ gameId: mockGame.id })
    );
  });

  it('onPlayerEdit dispatches editPlayer action', () => {
    spyOn(mockStore, 'dispatch');
    component.onPlayerEdit(mockPlayer);
    expect(mockStore.dispatch).toHaveBeenCalledWith(
      GameActions.editPlayer({ playerId: mockPlayer.id })
    );
  });

  it('onPlayerKick dispatches removePlayer action', () => {
    spyOn(mockStore, 'dispatch');
    component.onPlayerKick(mockPlayer);
    expect(mockStore.dispatch).toHaveBeenCalledWith(
      GameActions.removePlayer({ playerId: mockPlayer.id })
    );
  });

  it('onReorder dispatches reorderPlayers action', () => {
    spyOn(mockStore, 'dispatch');
    component.onReorder();
    expect(mockStore.dispatch).toHaveBeenCalledWith(GameActions.reorderPlayers());
  });
});
