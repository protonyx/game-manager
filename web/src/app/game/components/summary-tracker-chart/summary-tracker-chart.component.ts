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
          type: 'line'
        },
        title: undefined,
        xAxis: {
          title: {
            text: 'Time (minutes since game start)'
          },
          labels: {
            formatter: function() {
              // Format seconds as MM:SS
              const seconds = this.value as number;
              const minutes = Math.floor(seconds / 60);
              const remainingSeconds = Math.floor(seconds % 60);
              return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
            }
          },
          tickInterval: 30 // Show a tick every 30 seconds
        },
        yAxis: {
          title: {
            text: 'Value'
          }
        },
        tooltip: {
          formatter: function() {
            const seconds = this.x as number;
            const minutes = Math.floor(seconds / 60);
            const remainingSeconds = Math.floor(seconds % 60);
            const timeString = `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
            return `<b>${this.series.name}</b><br/>
                    Time: ${timeString}<br/>
                    Value: ${this.y}`;
          }
        },
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
