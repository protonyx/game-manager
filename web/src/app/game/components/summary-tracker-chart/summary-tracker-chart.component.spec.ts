import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { SummaryTrackerChartComponent } from './summary-tracker-chart.component';
import { PlayerSummary, TrackerSummary } from '../../models/models';

const makeTrackerSummary = (playerIds: string[]): TrackerSummary => ({
  id: 'tracker-1',
  name: 'HP',
  startingValue: 20,
  trackerHistory: playerIds.map((pid, i) => ({
    playerId: pid,
    trackerId: 'tracker-1',
    changedTime: new Date().toISOString(),
    newValue: 10 + i,
    secondsSinceGameStart: i * 30,
  })),
});

const makePlayerSummaries = (ids: string[]): PlayerSummary[] =>
  ids.map((id, i) => ({
    id,
    order: i + 1,
    name: `Player ${id}`,
    turns: [],
    trackerHistory: [],
  }));

describe('SummaryTrackerChartComponent', () => {
  let component: SummaryTrackerChartComponent;
  let fixture: ComponentFixture<SummaryTrackerChartComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SummaryTrackerChartComponent],
      schemas: [NO_ERRORS_SCHEMA],
    });
    fixture = TestBed.createComponent(SummaryTrackerChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('chartOptions is undefined initially', () => {
    expect(component.chartOptions).toBeUndefined();
  });

  it('builds chartOptions after setting tracker and players', () => {
    component.tracker = makeTrackerSummary(['p1', 'p2']);
    component.players = makePlayerSummaries(['p1', 'p2']);
    component.ngOnChanges();
    expect(component.chartOptions).toBeDefined();
  });

  it('creates one series entry per player', () => {
    component.tracker = makeTrackerSummary(['p1', 'p2']);
    component.players = makePlayerSummaries(['p1', 'p2']);
    component.ngOnChanges();
    expect((component.chartOptions!.series as any[]).length).toBe(2);
  });

  it('series names match player names', () => {
    component.tracker = makeTrackerSummary(['p1', 'p2']);
    component.players = makePlayerSummaries(['p1', 'p2']);
    component.ngOnChanges();
    const seriesNames = (component.chartOptions!.series as any[]).map((s) => s.name);
    expect(seriesNames).toEqual(['Player p1', 'Player p2']);
  });
});
