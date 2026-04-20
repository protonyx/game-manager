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

  it('selectColor() does nothing when color is taken', () => {
    setupComponent();
    component.takenColors = ['#1976D2'];

    component.selectColor('#1976D2');

    expect(component.playerForm.value.color).toBe('#D32F2F');
  });

  it('isColorSelected() returns true for the currently selected color', () => {
    setupComponent();
    component.playerForm.patchValue({ color: '#388E3C' });

    expect(component.isColorSelected('#388E3C')).toBeTrue();
  });

  it('isColorSelected() returns false for a different color', () => {
    setupComponent();
    component.playerForm.patchValue({ color: '#388E3C' });

    expect(component.isColorSelected('#D32F2F')).toBeFalse();
  });

  it('isColorSelected() is case-insensitive', () => {
    setupComponent();
    component.playerForm.patchValue({ color: '#D32F2F' });

    expect(component.isColorSelected('#d32f2f')).toBeTrue();
  });

  it('isColorTaken() returns true for a color in takenColors', () => {
    setupComponent();
    component.takenColors = ['#7B1FA2', '#C2185B'];

    expect(component.isColorTaken('#7B1FA2')).toBeTrue();
  });

  it('isColorTaken() returns false when color is not taken', () => {
    setupComponent();
    component.takenColors = ['#7B1FA2'];

    expect(component.isColorTaken('#D32F2F')).toBeFalse();
  });

  it('isColorTaken() is case-insensitive', () => {
    setupComponent();
    component.takenColors = ['#7B1FA2'];

    expect(component.isColorTaken('#7b1fa2')).toBeTrue();
  });
});
