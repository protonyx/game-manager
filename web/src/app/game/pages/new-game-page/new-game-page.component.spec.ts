import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { NewGamePageComponent } from './new-game-page.component';
import { GameActions } from '../../state/game.actions';
import { NewGame } from '../../models/models';

describe('NewGamePageComponent', () => {
  let component: NewGamePageComponent;
  let fixture: ComponentFixture<NewGamePageComponent>;
  let mockStore: MockStore;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewGamePageComponent, NoopAnimationsModule],
      providers: [provideMockStore()],
    }).compileComponents();

    mockStore = TestBed.inject(MockStore);
    fixture = TestBed.createComponent(NewGamePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('onCreateGame dispatches createGame action', () => {
    spyOn(mockStore, 'dispatch');
    const game: NewGame = {
      name: 'Test Game',
      options: { shareOtherPlayerTrackers: false },
      trackers: [],
    };

    component.onCreateGame(game);

    expect(mockStore.dispatch).toHaveBeenCalledWith(
      GameActions.createGame({ game })
    );
  });
});
