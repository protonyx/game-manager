import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { TrackerListComponent } from './tracker-list.component';
import { TrackerValue } from '../../models/models';

describe('TrackerListComponent', () => {
  let component: TrackerListComponent;
  let fixture: ComponentFixture<TrackerListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackerListComponent, NoopAnimationsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(TrackerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('emits the correct TrackerValue when onTrackerChange() is called', () => {
    const emitted: TrackerValue[] = [];
    component.updateTrackers.subscribe((v) => emitted.push(v));
    component.onTrackerChange('t1', 42);
    expect(emitted.length).toBe(1);
    expect(emitted[0]).toEqual({ trackerId: 't1', value: 42 });
  });
});
