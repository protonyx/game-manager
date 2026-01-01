import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { GamePageComponent } from './game-page.component';

describe('GamePageComponent', () => {
  let component: GamePageComponent;
  let fixture: ComponentFixture<GamePageComponent>;

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

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GamePageComponent, NoopAnimationsModule],
      providers: [provideMockStore({ initialState })],
    }).compileComponents();

    fixture = TestBed.createComponent(GamePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
