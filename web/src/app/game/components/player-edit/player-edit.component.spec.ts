import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { PlayerEditComponent } from './player-edit.component';
import { Player, Tracker } from '../../models/models';
import { PLAYER_COLORS } from '../../models/player-colors';

const mockPlayer: Player = {
  id: 'p1',
  name: 'Alice',
  color: '#D32F2F',
  order: 1,
  state: 'Active',
  trackerValues: { t1: 10 },
};

const mockTrackers: Tracker[] = [{ id: 't1', name: 'Score', startingValue: 0 }];

describe('PlayerEditComponent', () => {
  let component: PlayerEditComponent;
  let fixture: ComponentFixture<PlayerEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlayerEditComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(PlayerEditComponent);
    component = fixture.componentInstance;
    // Do NOT call fixture.detectChanges() here since player is null
  });

  function setupComponent(): void {
    component.player = mockPlayer;
    component.trackers = mockTrackers;
    component.ngOnChanges(); // must be called manually since inputs are set directly
    fixture.detectChanges();
  }

  it('should create', () => {
    setupComponent();
    expect(component).toBeTruthy();
  });

  it('populates name from player input after ngOnChanges', () => {
    setupComponent();

    expect(component.playerForm.value.name).toBe('Alice');
  });

  it('creates tracker control for each tracker', () => {
    setupComponent();

    const trackersGroup = component.playerForm.get('trackers') as import('@angular/forms').FormGroup;
    expect(trackersGroup.contains('t1')).toBeTrue();
  });

  it('updateTrackers(null) is a no-op', () => {
    setupComponent();

    const before = component.playerForm.get('trackers');
    component.updateTrackers(null);
    expect(component.playerForm.get('trackers')).toBe(before);
  });

  it('populates color from player input after ngOnChanges', () => {
    setupComponent();

    expect(component.playerForm.value.color).toBe('#D32F2F');
  });

  it('renders a swatch button for each available color', () => {
    setupComponent();

    const swatches = fixture.nativeElement.querySelectorAll('button.color-swatch');
    expect(swatches.length).toBe(PLAYER_COLORS.length);
  });

  it('selectColor() updates the form color value', () => {
    setupComponent();

    component.selectColor('#1976D2');

    expect(component.playerForm.value.color).toBe('#1976D2');
  });

  it('selectColor() updates the form color value regardless of takenColors (taken guard is in ColorPickerComponent)', () => {
    setupComponent();
    component.takenColors = ['#1976D2'];

    component.selectColor('#1976D2');

    expect(component.playerForm.value.color).toBe('#1976D2');
  });
});
