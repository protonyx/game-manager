import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import {
  selectSummaryName,
  selectSummaryPlayers,
  selectSummaryTrackers,
} from '../../state/game.reducer';
import { map } from 'rxjs';
import { PlayerSummary } from '../../models/models';
import { MatCardModule } from '@angular/material/card';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { SummaryTrackerChartComponent } from '../../components/summary-tracker-chart/summary-tracker-chart.component';
import { CdkTableDataSourceInput } from '@angular/cdk/table';

@Component({
  selector: 'app-game-summary-page',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    SummaryTrackerChartComponent,
  ],
  templateUrl: './game-summary-page.component.html',
  styleUrls: ['./game-summary-page.component.scss'],
})
export class GameSummaryPageComponent {
  title$ = this.store.select(selectSummaryName);

  trackers$ = this.store.select(selectSummaryTrackers);

  players$ = this.store.select(selectSummaryPlayers);

  turnSummaryDataSource$ = this.players$.pipe(
    map((data) => {
      return new MatTableDataSource<PlayerSummary>(
        <PlayerSummary[]>data,
      ) as CdkTableDataSourceInput<PlayerSummary>;
    }),
  );

  constructor(private store: Store) {}

  formatTurnDuration(turnDuration: number) {
    let totalSeconds = turnDuration;
    let hours = 0;
    let minutes = 0;

    if (totalSeconds >= 3600) {
      hours = Math.floor(totalSeconds / 3600);
      totalSeconds -= 3600 * hours;
    }

    if (totalSeconds >= 60) {
      minutes = Math.floor(totalSeconds / 60);
      totalSeconds -= 60 * minutes;
    }

    const seconds = totalSeconds;

    return `${hours}:${minutes}:${seconds.toFixed(0)}`;
  }
}
