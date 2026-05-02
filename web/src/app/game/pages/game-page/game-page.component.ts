import { Component, OnDestroy, OnInit } from '@angular/core';
import { createSelector, Store } from '@ngrx/store';
import { Router } from '@angular/router';
import {
  selectCurrentPlayer,
  selectCurrentPlayerIsHost,
  selectCurrentPlayerIsObserver,
  selectGame,
  selectAllPlayers,
  selectGameTrackers,
  selectCurrentPlayerId,
  selectTakenColors,
} from '../../state/game.selectors';
import { GameActions, GamesApiActions } from '../../state/game.actions';
import { Game, Player, TrackerValue } from '../../models/models';
import { TrackerListComponent } from '../../components/tracker-list/tracker-list.component';
import { CommonModule } from '@angular/common';
import { CurrentTurnComponent } from '../../components/current-turn/current-turn.component';
import { PlayerListComponent } from '../../components/player-list/player-list.component';
import { MatButtonModule } from '@angular/material/button';
import { LetDirective } from '@ngrx/component';
import { Observable, map, switchMap, of, combineLatest } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';
import { TurnTimerComponent } from '../../components/turn-timer/turn-timer.component';
import { PatchOperation } from '../../models/patch';
import { HostLobbyComponent } from '../../components/host-lobby/host-lobby.component';
import { PlayerWaitingComponent } from '../../components/player-waiting/player-waiting.component';
import { ObserverWaitingComponent } from '../../components/observer-waiting/observer-waiting.component';

const selectIsCurrentPlayerTurn = createSelector(
  selectCurrentPlayerId,
  selectGame,
  (currentPlayerId, game) => game?.currentTurnPlayerId === currentPlayerId,
);

@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss'],
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    PlayerListComponent,
    CurrentTurnComponent,
    TrackerListComponent,
    TurnTimerComponent,
    LetDirective,
    HostLobbyComponent,
    PlayerWaitingComponent,
    ObserverWaitingComponent,
  ],
})
export class GamePageComponent implements OnInit, OnDestroy {
  currentPlayer$ = this.store.select(selectCurrentPlayer);

  game$ = this.store.select(selectGame);

  trackers$ = this.store.select(selectGameTrackers);

  players$ = this.store.select(selectAllPlayers);

  isHost$ = this.store.select(selectCurrentPlayerIsHost);

  isObserver$ = this.store.select(selectCurrentPlayerIsObserver);

  isMyTurn$ = this.store.select(selectIsCurrentPlayerTurn);

  takenColors$: Observable<string[]> = this.store
    .select(selectCurrentPlayerId)
    .pipe(
      switchMap((playerId) =>
        playerId ? this.store.select(selectTakenColors(playerId)) : of([]),
      ),
    );

  currentTurnPlayer$: Observable<Player | undefined> = combineLatest([
    this.game$,
    this.players$
  ]).pipe(
    map(([game, players]) => {
      if (!players || !game?.currentTurnPlayerId) return undefined;
      return players.find(p => p.id === game.currentTurnPlayerId);
    })
  );

  nextTurnPlayer$: Observable<Player | undefined> = combineLatest([
    this.game$,
    this.players$
  ]).pipe(
    map(([game, players]) => {
      if (!players || !game?.currentTurnPlayerId) return undefined;
      const idx = players.findIndex(p => p.id === game.currentTurnPlayerId);
      if (idx < 0) return undefined;
      return players[(idx + 1) % players.length];
    })
  );

  fabOpen = false;

  lockResolver: ((value: PromiseLike<unknown> | unknown) => void) | undefined;

  constructor(
    private store: Store,
    private router: Router,
  ) {}

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
    // Release web lock
    if (this.lockResolver) {
      this.lockResolver(null);
    }
  }

  onEndTurn(game: Game | null | undefined): void {
    this.store.dispatch(GameActions.endTurn({ gameId: game!.id }));
  }

  onStartGame(game: Game | null | undefined): void {
    this.store.dispatch(GameActions.startGame({ gameId: game!.id }));
  }

  onEndGame(game: Game | null | undefined) {
    this.store.dispatch(GameActions.endGame({ gameId: game!.id }));
  }

  onReorder(): void {
    this.store.dispatch(GameActions.reorderPlayers());
  }

  async onLeave(): Promise<void> {
    this.store.dispatch(GameActions.leaveGame());
  }

  onLeaveGame(): void {
    this.store.dispatch(GameActions.leaveGame());
    this.router.navigate(['/game', 'join']);
  }

  onPlayerEdit(player: Player): void {
    this.store.dispatch(GameActions.editPlayer({ playerId: player.id }));
  }

  onPlayerKick(player: Player): void {
    this.store.dispatch(GameActions.removePlayer({ playerId: player.id }));
  }

  onTrackerUpdate(player: Player, trackerValue: TrackerValue): void {
    this.store.dispatch(
      GameActions.updateTracker({
        playerId: player.id,
        tracker: trackerValue,
      }),
    );
  }

  onEditTracker(event: { playerId: string; trackerId: string }) {
    this.store.dispatch(
      GameActions.editTracker({ playerId: event.playerId, trackerId: event.trackerId })
    );
  }

  onReadyToggled(player: Player, isReady: boolean): void {
    this.store.dispatch(GameActions.setPlayerReady({ playerId: player.id, isReady }));
  }

  onPlayerPatched(event: { playerId: string; ops: PatchOperation[] }): void {
    this.store.dispatch(
      GamesApiActions.patchPlayer({ playerId: event.playerId, ops: event.ops }),
    );
  }
}
