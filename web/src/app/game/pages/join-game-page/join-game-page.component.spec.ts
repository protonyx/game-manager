import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { provideMockActions } from '@ngrx/effects/testing';
import { of } from 'rxjs';

import { JoinGamePageComponent } from './join-game-page.component';
import { GameActions } from '../../state/game.actions';
import { JoinGame } from '../../models/models';

describe('JoinGamePageComponent', () => {
  let component: JoinGamePageComponent;
  let fixture: ComponentFixture<JoinGamePageComponent>;
  let mockStore: MockStore;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JoinGamePageComponent, NoopAnimationsModule],
      providers: [
        provideMockStore(),
        provideMockActions(() => of()),
      ],
    }).compileComponents();

    mockStore = TestBed.inject(MockStore);
    fixture = TestBed.createComponent(JoinGamePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('onJoinGame dispatches joinGame action', () => {
    spyOn(mockStore, 'dispatch');
    const joinGame: JoinGame = { entryCode: 'ABCD', name: 'Player 1', observer: false };

    component.onJoinGame(joinGame);

    expect(mockStore.dispatch).toHaveBeenCalledWith(
      GameActions.joinGame({ joinGame })
    );
  });

  it('onJoinGame sets loading to true', () => {
    const joinGame: JoinGame = { entryCode: 'ABCD', name: 'Player 1', observer: false };

    component.onJoinGame(joinGame);

    expect(component.loading).toBeTrue();
  });
});
