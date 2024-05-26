import { Component, OnDestroy, OnInit } from '@angular/core';
import { createSelector, Store } from '@ngrx/store';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { combineLatest, map, Subject, takeUntil, tap } from 'rxjs';
import {
  selectCurrentPlayer,
  selectCurrentPlayerIsHost,
  selectGame,
  selectAllPlayers, selectGameTrackers, selectCurrentPlayerId
} from '../../state/game.reducer';
import { GameActions } from '../../state/game.actions';
import {
  Game,
  Player,
  PlayerCredentials,
  Tracker,
  TrackerValue,
} from '../../models/models';
import { TrackerEditorComponent } from '../../components/tracker-editor/tracker-editor.component';
import { CommonModule } from '@angular/common';
import { CurrentTurnComponent } from '../../components/current-turn/current-turn.component';
import { PlayerListComponent } from '../../components/player-list/player-list.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { GameControlComponent } from '../../components/game-control/game-control.component';
import { MatButtonModule } from '@angular/material/button';
import { PlayerReorderModalComponent } from '../../components/player-reorder-modal/player-reorder-modal.component';

const selectIsCurrentPlayerTurn = createSelector(
  selectCurrentPlayerId,
  selectGame,
  (currentPlayerId, game) => game?.currentTurnPlayerId === currentPlayerId
);

@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatDialogModule,
    MatExpansionModule,
    GameControlComponent,
    PlayerListComponent,
    CurrentTurnComponent,
    TrackerEditorComponent,
  ],
})
export class GamePageComponent implements OnInit, OnDestroy {
  currentPlayer$ = this.store.select(selectCurrentPlayer);

  game$ = this.store.select(selectGame).pipe(tap((g) => (this.game = g)));

  trackers$ = this.store.select(selectGameTrackers);

  players$ = this.store.select(selectAllPlayers);

  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  isHost$ = this.store.select(selectCurrentPlayerIsHost);

  isMyTurn$ = this.store.select(selectIsCurrentPlayerTurn);

  game: Game | null | undefined;

  players: Player[] | null | undefined;

  currentPlayer: Player | null | undefined;

  trackers: Tracker[] | null | undefined;

  lockResolver: ((value: PromiseLike<unknown> | unknown) => void) | undefined;

  constructor(
    private store: Store,
    private dialog: MatDialog,
  ) {
    this.trackers$.pipe(takeUntil(this.unsubscribe$)).subscribe((data) => {
      this.trackers = data;
    });
    this.players$.pipe(takeUntil(this.unsubscribe$)).subscribe((data) => {
      this.players = data;
    });
    this.currentPlayer$.pipe(takeUntil(this.unsubscribe$)).subscribe((data) => {
      this.currentPlayer = data;
    });
  }

  ngOnInit(): void {
    // Request a web lock to prevent tab from sleeping
    if (navigator && navigator.locks && navigator.locks.request) {
      const promise = new Promise((res) => {
        this.lockResolver = res;
      });

      navigator.locks.request('game-manager', { mode: 'shared' }, () => {
        return promise;
      });
    }
  }

  ngOnDestroy() {
    this.unsubscribe$.next(true);
    this.unsubscribe$.unsubscribe();

    // Release web lock
    if (this.lockResolver) {
      this.lockResolver(null);
    }
  }

  onEndTurn(game: Game | null | undefined): void {
    this.store.dispatch(GameActions.endTurn({ gameId: game!.id }));
  }

  onStartGame(): void {
    this.store.dispatch(GameActions.startGame({ gameId: this.game!.id }));
  }

  onEndGame() {
    this.store.dispatch(GameActions.endGame({ gameId: this.game!.id }));
  }

  onReorder(): void {
    const dialogRef = this.dialog.open(PlayerReorderModalComponent, {
      data: {
        players: this.players,
      },
      width: '400px',
    });

    dialogRef.afterClosed().subscribe((data: Player[]) => {
      if (data) {
        this.store.dispatch(
          GameActions.updatePlayerOrder({
            gameId: this.game!.id,
            players: data,
          }),
        );
      }
    });
  }

  async onLeave(): Promise<void> {
    this.store.dispatch(GameActions.leaveGame());
  }

  onPlayerEdit(player: Player): void {
    this.store.dispatch(GameActions.editPlayer({playerId: player.id }));
  }

  onTrackerUpdate(trackerValue: TrackerValue): void {
    if (this.currentPlayer) {
      this.store.dispatch(
        GameActions.updateTracker({
          playerId: this.currentPlayer.id,
          tracker: trackerValue,
        }),
      );
    }
  }
}
