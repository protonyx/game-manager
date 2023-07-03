import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Actions, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { combineLatest, filter, map, Subject, takeUntil, tap } from 'rxjs';
import {
  selectCredentials,
  selectCurrentPlayer,
  selectGame,
  selectPlayers,
} from '../../state/game.reducer';
import {
  GameActions,
  GameHubActions,
  GamesApiActions,
} from '../../state/game.actions';
import { LayoutActions } from '../../../shared/state/layout.actions';
import { GameHubService } from '../../services/game-hub.service';
import { GameService } from '../../services/game.service';
import {
  Game,
  Player,
  PlayerCredentials,
  Tracker,
  TrackerValue,
} from '../../models/models';
import { PlayerEditComponent } from '../../components/player-edit/player-edit.component';
import { TrackerEditorComponent } from '../../components/tracker-editor/tracker-editor.component';
import { NgIf, AsyncPipe } from '@angular/common';
import { CurrentTurnComponent } from '../../components/current-turn/current-turn.component';
import { PlayerListComponent } from '../../components/player-list/player-list.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { GameControlComponent } from '../../components/game-control/game-control.component';

@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss'],
  standalone: true,
  imports: [
    MatDialogModule,
    MatExpansionModule,
    MatSnackBarModule,
    GameControlComponent,
    PlayerListComponent,
    CurrentTurnComponent,
    NgIf,
    TrackerEditorComponent,
    AsyncPipe,
  ],
})
export class GamePageComponent implements OnInit, OnDestroy {
  credentials$ = this.store.select(selectCredentials);

  currentPlayer$ = this.store.select(selectCurrentPlayer);

  game$ = this.store.select(selectGame).pipe(tap((g) => (this.game = g)));

  trackers$ = this.game$.pipe(map((g) => g?.trackers));

  players$ = this.store.select(selectPlayers);

  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  isAdmin$ = this.store
    .select(selectCredentials)
    .pipe(map((c) => c?.isAdmin || false));

  isMyTurn$ = combineLatest({
    game: this.game$,
    currentPlayer: this.currentPlayer$,
  }).pipe(
    map(
      ({ game, currentPlayer }) =>
        game?.currentTurnPlayerId === currentPlayer?.id
    )
  );

  credentials: PlayerCredentials | undefined;

  game: Game | null | undefined;

  trackers: Tracker[] | null | undefined;

  constructor(
    private gameService: GameService,
    private signalr: GameHubService,
    private store: Store,
    private actions$: Actions,
    private router: Router,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.trackers$.pipe(takeUntil(this.unsubscribe$)).subscribe((data) => {
      this.trackers = data;
    });
  }

  ngOnInit(): void {
    this.credentials$
      .pipe(
        takeUntil(this.unsubscribe$),
        filter((credentials) => credentials != null)
      )
      .subscribe((credentials) => {
        this.credentials = credentials ? credentials : undefined;

        this.onRefresh();
        this.connect(credentials!);
      });

    // Update title and entry code when game is loaded
    this.actions$
      .pipe(ofType(GamesApiActions.retrievedGame), takeUntil(this.unsubscribe$))
      .subscribe((data) => {
        this.store.dispatch(LayoutActions.setTitle({ title: data.game.name }));
        this.store.dispatch(
          LayoutActions.setEntryCode({ entryCode: data.game.entryCode })
        );
      });

    // Handle SignalR disconnects
    this.actions$
      .pipe(
        ofType(GameHubActions.hubDisconnected),
        takeUntil(this.unsubscribe$)
      )
      .subscribe((connected) => {
        let snackBarRef = this.snackBar.open(
          'Server Disconnected',
          'Reconnect'
        );
        snackBarRef.onAction().subscribe(() => {
          this.signalr.reconnect();
          this.onRefresh();
        });
      });
  }

  ngOnDestroy() {
    this.unsubscribe$.next(true);
    this.unsubscribe$.unsubscribe();

    this.signalr.disconnect();
  }

  onEndTurn(): void {
    this.signalr.endTurn();
  }

  onStartGame(): void {
    this.signalr.endTurn();
  }

  onRefresh(): void {
    if (this.credentials) {
      this.store.dispatch(
        GameActions.loadGame({ gameId: this.credentials.gameId })
      );
      this.store.dispatch(
        GameActions.loadCurrentPlayer({ playerId: this.credentials.playerId })
      );
      this.store.dispatch(
        GameActions.loadPlayers({ gameId: this.credentials.gameId })
      );
    }
  }

  async onLeave(): Promise<void> {
    await this.signalr.disconnect();
    this.store.dispatch(GameActions.leaveGame());

    await this.router.navigate(['./join']);
  }

  onPlayerOrderUpdated(player: Player): void {
    this.store.dispatch(
      GameActions.updatePlayerOrder({
        playerId: player.id,
        order: player.order,
      })
    );
  }

  onPlayerEdit(player: Player): void {
    const dialogRef = this.dialog.open(PlayerEditComponent, {
      data: {
        player: player,
        trackers: this.trackers,
      },
    });

    dialogRef.afterClosed().subscribe((data) => {
      if (data) {
        const ops = [{ op: 'replace', path: '/name', value: data.name }];
        if (this.trackers && this.trackers.length > 0) {
          this.trackers.forEach((t) => {
            ops.push({
              op: 'replace',
              path: `/trackerValues/${t.id}`,
              value: data.trackers[t.id],
            });
          });
        }
        this.gameService.patchPlayer(player.id, ops).subscribe();
      }
    });
  }

  onTrackerUpdate(trackerValue: TrackerValue): void {
    this.store.dispatch(GameActions.updateTracker({ tracker: trackerValue }));
  }

  onPlayerKick(player: Player): void {
    this.store.dispatch(GameActions.removePlayer({ playerId: player.id }));
  }

  private connect(credentials: PlayerCredentials) {
    this.signalr.connect(credentials!.gameId, credentials!.token);
  }
}
