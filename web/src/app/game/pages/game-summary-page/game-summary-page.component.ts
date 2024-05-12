import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { createSelector, Store } from '@ngrx/store';
import { GameActions } from '../../state/game.actions';
import {
  selectSummary,
  selectSummaryPlayers,
  selectSummaryTrackers,
} from '../../state/game.reducer';
import { getRouterSelectors } from '@ngrx/router-store';
import { ActivatedRoute } from '@angular/router';
import { filter, map } from 'rxjs';
import { PlayerSummary } from '../../models/models';
import { MatCardModule } from '@angular/material/card';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { SummaryTrackerChartComponent } from '../../components/summary-tracker-chart/summary-tracker-chart.component';
import { CdkTableDataSourceInput } from '@angular/cdk/table';

const { selectRouteParams } = getRouterSelectors();

const selectGameId = createSelector(selectRouteParams, ({ id }) =>
  id.toString(),
);

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
export class GameSummaryPageComponent implements OnInit {
  gameSummary$ = this.store.select(selectSummary);

  title$ = this.gameSummary$.pipe(map((summary) => summary?.name));

  trackers$ = this.store.select(selectSummaryTrackers);

  players$ = this.store.select(selectSummaryPlayers);

  turnSummaryDataSource$ = this.players$.pipe(
    map((data) => {
      return new MatTableDataSource<PlayerSummary>(
        <PlayerSummary[]>data,
      ) as CdkTableDataSourceInput<PlayerSummary>;
    }),
  );

  gameId$ = this.store.select(selectGameId);

  constructor(
    private store: Store,
    private route: ActivatedRoute,
  ) {}

  ngOnInit() {
    this.route.paramMap
      .pipe(
        filter((params) => params.has('id')),
        map((params) => params.get('id')),
      )
      .subscribe((id) => {
        this.store.dispatch(GameActions.loadGameSummary({ gameId: id! }));
      });
  }

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
