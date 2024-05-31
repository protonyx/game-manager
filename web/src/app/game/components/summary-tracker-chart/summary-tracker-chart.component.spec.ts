import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummaryTrackerChartComponent } from './summary-tracker-chart.component';

describe('SummaryTrackerChartComponent', () => {
  let component: SummaryTrackerChartComponent;
  let fixture: ComponentFixture<SummaryTrackerChartComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SummaryTrackerChartComponent],
    });
    fixture = TestBed.createComponent(SummaryTrackerChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
