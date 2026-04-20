import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { PlayerEditComponent } from './player-edit.component';
import { Player, Tracker } from '../../models/models';

const mockPlayer: Player = {
  id: 'p1',
  name: 'Alice',
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
});
