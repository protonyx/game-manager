import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HostLobbyComponent } from './host-lobby.component';
import { Game, GameState, Player } from '../../models/models';

const makePlayer = (id: string, overrides: Partial<Player> = {}): Player => ({
  id,
  order: 1,
  name: `Player ${id}`,
  color: '#D32F2F',
  state: 'Active',
  trackerValues: {},
  ...overrides,
});

const makeGame = (): Game => ({
  id: 'g1',
  name: 'Test Game',
  entryCode: 'ABCD',
  state: GameState.Preparing,
  trackers: [],
  options: { shareOtherPlayerTrackers: false },
  createdDate: new Date().toISOString(),
});

describe('HostLobbyComponent', () => {
  let component: HostLobbyComponent;
  let fixture: ComponentFixture<HostLobbyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostLobbyComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(HostLobbyComponent);
    component = fixture.componentInstance;
    component.game = makeGame();
    component.currentPlayer = makePlayer('host');
    component.players = [];
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('allReady', () => {
    it('returns false when there are no non-host players', () => {
      component.players = [makePlayer('host')];
      expect(component.allReady).toBeFalse();
    });

    it('returns false when some non-host players are not ready', () => {
      component.players = [
        makePlayer('host'),
        makePlayer('p2', { isReady: true }),
        makePlayer('p3', { isReady: false }),
      ];
      expect(component.allReady).toBeFalse();
    });

    it('returns true when all non-host players are ready', () => {
      component.players = [
        makePlayer('host'),
        makePlayer('p2', { isReady: true }),
        makePlayer('p3', { isReady: true }),
      ];
      expect(component.allReady).toBeTrue();
    });
  });

  describe('Start Game button', () => {
    it('is disabled when not all players are ready', () => {
      component.players = [
        makePlayer('host'),
        makePlayer('p2', { isReady: false }),
      ];
      fixture.detectChanges();
      const btn: HTMLButtonElement = fixture.nativeElement.querySelector('.start-btn');
      expect(btn.disabled).toBeTrue();
    });

    it('is enabled when all non-host players are ready', () => {
      component.players = [
        makePlayer('host'),
        makePlayer('p2', { isReady: true }),
      ];
      fixture.detectChanges();
      const btn: HTMLButtonElement = fixture.nativeElement.querySelector('.start-btn');
      expect(btn.disabled).toBeFalse();
    });
  });

  describe('startGame output', () => {
    it('emits startGame when Start Game button is clicked and all ready', () => {
      spyOn(component.startGame, 'emit');
      component.players = [
        makePlayer('host'),
        makePlayer('p2', { isReady: true }),
      ];
      fixture.detectChanges();
      fixture.nativeElement.querySelector('.start-btn').click();
      expect(component.startGame.emit).toHaveBeenCalled();
    });

    it('emits startGame from onStartAnyways after confirm', () => {
      spyOn(window, 'confirm').and.returnValue(true);
      spyOn(component.startGame, 'emit');
      component.onStartAnyways();
      expect(component.startGame.emit).toHaveBeenCalled();
    });

    it('does not emit from onStartAnyways when confirm is cancelled', () => {
      spyOn(window, 'confirm').and.returnValue(false);
      spyOn(component.startGame, 'emit');
      component.onStartAnyways();
      expect(component.startGame.emit).not.toHaveBeenCalled();
    });
  });

  describe('editPlayer output', () => {
    it('emits editPlayer when edit button is clicked', () => {
      spyOn(component.editPlayer, 'emit');
      const player = makePlayer('p1');
      component.players = [player];
      fixture.detectChanges();
      const editBtn: HTMLButtonElement = fixture.nativeElement.querySelector('.edit-btn');
      editBtn.click();
      expect(component.editPlayer.emit).toHaveBeenCalledWith(player);
    });
  });

  describe('playerCount', () => {
    it('returns number of players', () => {
      component.players = [makePlayer('p1'), makePlayer('p2')];
      expect(component.playerCount).toBe(2);
    });
  });
});
