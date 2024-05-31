import { Component, OnDestroy, OnInit } from '@angular/core';
import { createSelector, Store } from '@ngrx/store';
import {
  selectCurrentPlayer,
  selectCurrentPlayerIsHost,
  selectGame,
  selectAllPlayers,
  selectGameTrackers,
  selectCurrentPlayerId,
} from '../../state/game.selectors';
import { GameActions } from '../../state/game.actions';
import { Game, Player, TrackerValue } from '../../models/models';
import { TrackerEditorComponent } from '../../components/tracker-editor/tracker-editor.component';
import { CommonModule } from '@angular/common';
import { CurrentTurnComponent } from '../../components/current-turn/current-turn.component';
import { PlayerListComponent } from '../../components/player-list/player-list.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { GameControlComponent } from '../../components/game-control/game-control.component';
import { MatButtonModule } from '@angular/material/button';
import { LetDirective } from '@ngrx/component';

const selectIsCurrentPlayerTurn = createSelector(
  selectCurrentPlayerId,
  selectGame,
  (currentPlayerId, game) => game?.currentTurnPlayerId === currentPlayerId,
);

@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatExpansionModule,
    GameControlComponent,
    PlayerListComponent,
    CurrentTurnComponent,
    TrackerEditorComponent,
    LetDirective,
  ],
})
export class GamePageComponent implements OnInit, OnDestroy {
  currentPlayer$ = this.store.select(selectCurrentPlayer);

  game$ = this.store.select(selectGame);

  trackers$ = this.store.select(selectGameTrackers);

  players$ = this.store.select(selectAllPlayers);

  isHost$ = this.store.select(selectCurrentPlayerIsHost);

  isMyTurn$ = this.store.select(selectIsCurrentPlayerTurn);

  lockResolver: ((value: PromiseLike<unknown> | unknown) => void) | undefined;

  constructor(private store: Store) {}

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

  onPlayerEdit(player: Player): void {
    this.store.dispatch(GameActions.editPlayer({ playerId: player.id }));
  }

  onTrackerUpdate(player: Player, trackerValue: TrackerValue): void {
    this.store.dispatch(
      GameActions.updateTracker({
        playerId: player.id,
        tracker: trackerValue,
      }),
    );
  }
}
