import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { PlayerWaitingComponent } from './player-waiting.component';
import { Game, GameState, Player } from '../../models/models';

const makePlayer = (overrides: Partial<Player> = {}): Player => ({
  id: 'p1',
  order: 1,
  name: 'Alice',
  color: '#D32F2F',
  state: 'Active',
  trackerValues: {},
  ...overrides,
});

const makeGame = (): Game => ({
  id: 'g1',
  name: 'Awesome Game',
  entryCode: 'ZXCV',
  state: GameState.Preparing,
  trackers: [],
  options: { shareOtherPlayerTrackers: false },
  createdDate: new Date().toISOString(),
});

describe('PlayerWaitingComponent', () => {
  let component: PlayerWaitingComponent;
  let fixture: ComponentFixture<PlayerWaitingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlayerWaitingComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(PlayerWaitingComponent);
    component = fixture.componentInstance;
    component.game = makeGame();
    component.currentPlayer = makePlayer();
    component.ngOnChanges();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('renders the game name', () => {
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.game-name')?.textContent).toContain('Awesome Game');
  });

  it('renders the entry code', () => {
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.entry-code')?.textContent).toContain('ZXCV');
  });

  it('renders name input with player name', () => {
    expect(component.nameControl.value).toBe('Alice');
  });

  it('renders 12 color swatches', () => {
    const swatches = fixture.nativeElement.querySelectorAll('.color-swatch');
    expect(swatches.length).toBe(12);
  });

  it('renders the ready button', () => {
    const btn: HTMLElement = fixture.nativeElement.querySelector('.ready-btn');
    expect(btn).toBeTruthy();
    expect(btn.textContent).toContain("I'm Ready");
  });

  describe('readyToggled output', () => {
    it('emits true when player is not ready and button is clicked', () => {
      spyOn(component.readyToggled, 'emit');
      component.currentPlayer = makePlayer({ isReady: false });
      component.onReadyToggle();
      expect(component.readyToggled.emit).toHaveBeenCalledWith(true);
    });

    it('emits false when player is already ready and button is clicked', () => {
      spyOn(component.readyToggled, 'emit');
      component.currentPlayer = makePlayer({ isReady: true });
      component.onReadyToggle();
      expect(component.readyToggled.emit).toHaveBeenCalledWith(false);
    });
  });

  describe('playerPatched output', () => {
    it('emits playerPatched with name op on blur when name changed', () => {
      spyOn(component.playerPatched, 'emit');
      component.nameControl.setValue('Bob');
      component.nameControl.markAsDirty();
      component.onNameBlur();
      expect(component.playerPatched.emit).toHaveBeenCalledWith({
        playerId: 'p1',
        ops: [{ op: 'replace', path: '/name', value: 'Bob' }],
      });
    });

    it('does not emit playerPatched on blur when name unchanged', () => {
      spyOn(component.playerPatched, 'emit');
      component.nameControl.setValue('Alice');
      component.onNameBlur();
      expect(component.playerPatched.emit).not.toHaveBeenCalled();
    });

    it('emits playerPatched with color op when color selected', () => {
      spyOn(component.playerPatched, 'emit');
      component.onColorSelect('#1976D2');
      expect(component.playerPatched.emit).toHaveBeenCalledWith({
        playerId: 'p1',
        ops: [{ op: 'replace', path: '/color', value: '#1976D2' }],
      });
    });

    it('does not emit playerPatched when selecting the same color', () => {
      spyOn(component.playerPatched, 'emit');
      component.onColorSelect('#D32F2F'); // same as player.color
      expect(component.playerPatched.emit).not.toHaveBeenCalled();
    });
  });

  describe('color swatch states', () => {
    it('marks selected color correctly', () => {
      expect(component.isColorSelected('#D32F2F')).toBeTrue();
      expect(component.isColorSelected('#1976D2')).toBeFalse();
    });

    it('marks taken colors correctly', () => {
      component.takenColors = ['#1976D2'];
      expect(component.isColorTaken('#1976D2')).toBeTrue();
      expect(component.isColorTaken('#D32F2F')).toBeFalse();
    });

    it('does not emit when a taken color is selected', () => {
      spyOn(component.playerPatched, 'emit');
      component.takenColors = ['#1976D2'];
      component.onColorSelect('#1976D2');
      expect(component.playerPatched.emit).not.toHaveBeenCalled();
    });
  });
});
