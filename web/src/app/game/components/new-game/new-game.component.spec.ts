import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { NewGameComponent } from './new-game.component';
import { NewGame } from '../../models/models';

describe('NewGameComponent', () => {
  let component: NewGameComponent;
  let fixture: ComponentFixture<NewGameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewGameComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(NewGameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('form starts with empty trackers array', () => {
    expect(component.trackers.length).toBe(0);
  });

  it('addTracker() adds a tracker with default name "Score" and startingValue 0', () => {
    component.addTracker();

    expect(component.trackers.length).toBe(1);
    expect(component.trackerControls[0].value).toEqual({ name: 'Score', startingValue: 0 });
  });

  it('deleteTracker(0) removes the first tracker', () => {
    component.addTracker();
    component.addTracker();

    component.deleteTracker(0);

    expect(component.trackers.length).toBe(1);
  });

  it('onSubmit() emits newGame event with the form value', () => {
    component.gameForm.controls['name'].setValue('My Game');
    let emitted: NewGame | undefined;
    component.newGame.subscribe((v) => (emitted = v));

    component.onSubmit();

    expect(emitted?.name).toBe('My Game');
  });
});
