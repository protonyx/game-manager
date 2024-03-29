import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { GameActions } from '../../state/game.actions';
import { selectSummary } from '../../state/game.reducer';
import { ActivatedRoute } from '@angular/router';
import { filter, map, Subject } from 'rxjs';
import {
  PlayerSummary,
  PlayerTrackerHistory,
  TrackerSummary,
} from '../../models/models';
import { MatCardModule } from '@angular/material/card';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { SummaryTrackerChartComponent } from '../../components/summary-tracker-chart/summary-tracker-chart.component';

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
export class GameSummaryPageComponent implements OnInit, OnDestroy {
  gameSummary$ = this.store.select(selectSummary);

  title$ = this.gameSummary$.pipe(map((summary) => summary?.name));

  trackers$ = this.gameSummary$.pipe(
    map((summary) => {
      return summary?.trackers.map((tracker) => {
        return <TrackerSummary>{
          ...tracker,
          trackerHistory: summary.players.reduce((acc, player) => {
            // Starting value
            acc.push({
              playerId: player.id,
              trackerId: tracker.id,
              newValue: tracker.startingValue,
              secondsSinceGameStart: 0,
            } as PlayerTrackerHistory);

            for (const th of player.trackerHistory.filter(
              (th) => th.trackerId === tracker.id
            )) {
              acc.push({
                ...th,
                playerId: player.id,
              } as PlayerTrackerHistory);
            }
            return acc;
          }, new Array<PlayerTrackerHistory>()),
        };
      });
    })
  );

  players$ = this.gameSummary$.pipe(
    filter((summary) => !!summary),
    map((summary) => {
      return summary!.players.map((player) => {
        return {
          ...player,
          turnCount: player.turns.length,
          avgTurnDuration:
            player.turns
              .map((t) => t.durationSeconds)
              .reduce((acc, d) => (acc += d), 0) / player.turns.length,
        };
      });
    })
  );

  turnSummaryDataSource$ = this.players$.pipe(
    map((data) => {
      return new MatTableDataSource<PlayerSummary>(<PlayerSummary[]>data);
    })
  );

  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  constructor(private store: Store, private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.paramMap
      .pipe(
        filter((params) => params.has('id')),
        map((params) => params.get('id'))
      )
      .subscribe((id) => {
        this.store.dispatch(GameActions.loadGameSummary({ gameId: id! }));
      });
  }

  ngOnDestroy() {
    this.unsubscribe$.next(true);
    this.unsubscribe$.unsubscribe();
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
