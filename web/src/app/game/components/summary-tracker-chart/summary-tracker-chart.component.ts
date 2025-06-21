import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HighchartsChartModule } from 'highcharts-angular';
import * as Highcharts from 'highcharts';
import { PlayerSummary, TrackerSummary } from '../../models/models';

@Component({
    selector: 'app-summary-tracker-chart',
    imports: [CommonModule, HighchartsChartModule],
    templateUrl: './summary-tracker-chart.component.html',
    styleUrls: ['./summary-tracker-chart.component.scss']
})
export class SummaryTrackerChartComponent implements OnChanges {
  Highcharts: typeof Highcharts = Highcharts;

  @Input()
  public tracker: TrackerSummary | undefined;

  @Input()
  public players: PlayerSummary[] | undefined | null;

  chartOptions: Highcharts.Options | undefined;

  ngOnChanges(): void {
    if (!!this.tracker && !!this.players) {
      this.chartOptions = {
        chart: {
          height: '300px',
        },
        title: undefined,
        series: this.players!.map((p) => {
          return {
            type: 'line',
            step: 'left',
            name: p.name,
            data: this.tracker!.trackerHistory!.filter(
              (th) => th.playerId === p.id,
            ).map((th) => [th.secondsSinceGameStart, th.newValue]),
          };
        }),
      };
    }
  }
}
