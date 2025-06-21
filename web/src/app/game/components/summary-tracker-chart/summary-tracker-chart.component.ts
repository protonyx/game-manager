import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HighchartsChartComponent, ChartConstructorType } from 'highcharts-angular';
import { PlayerSummary, TrackerSummary } from '../../models/models';

@Component({
    selector: 'app-summary-tracker-chart',
    imports: [CommonModule, HighchartsChartComponent],
    templateUrl: './summary-tracker-chart.component.html',
    styleUrls: ['./summary-tracker-chart.component.scss']
})
export class SummaryTrackerChartComponent implements OnChanges {

  @Input()
  public tracker: TrackerSummary | undefined;

  @Input()
  public players: PlayerSummary[] | undefined | null;

  chartConstructor: ChartConstructorType = 'chart';

  chartOptions: Highcharts.Options | undefined;

  ngOnChanges(): void {
    if (!!this.tracker && !!this.players) {
      this.chartOptions = <Highcharts.Options>{
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
