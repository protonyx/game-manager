import { Component, inject, OnDestroy, OnInit } from '@angular/core';
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
import { PushSubscriptionService } from '../../services/push-subscription.service';
import { PushInstructionsDialogComponent } from '../../dialogs/push-instructions-dialog/push-instructions-dialog.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { InstallPromptService } from '../../../shared/services/install-prompt.service';

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
    MatDialogModule,
    MatSnackBarModule,
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
  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly pushSubscription = inject(PushSubscriptionService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly installPrompt = inject(InstallPromptService);

  readonly pushSupported = this.pushSubscription.isSupported();
  readonly showIosInstallGuide =
    this.pushSubscription.isIos() && !this.pushSubscription.isStandalone();
  readonly canInstall$ = this.installPrompt.available$;
  pushEnabled = false;
  pushPermission: NotificationPermission = 'default';

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

  ngOnInit(): void {
    this.pushPermission = this.pushSubscription.permission();
    void this.refreshPushStatus().catch((error) => {
      console.error('Could not read push notification status.', error);
    });

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

  async onPushToggle(): Promise<void> {
    if (!this.pushSupported) return;

    if (this.pushEnabled) {
      try {
        await this.pushSubscription.unsubscribe();
        this.pushEnabled = false;
      } catch (error) {
        console.error('Could not disable turn notifications.', error);
        this.snackBar.open('Could not disable turn notifications. Please try again.', 'Dismiss', {
          duration: 5000,
        });
      }
      return;
    }

    if (this.pushSubscription.isIos() && !this.pushSubscription.isStandalone()) {
      this.showPushInstructions(
        'Add Game Manager to your Home Screen',
        "To get turn notifications on iPhone or iPad, tap the Share button and choose 'Add to Home Screen', then open Game Manager from your Home Screen.",
      );
      return;
    }

    if (this.pushSubscription.permission() === 'denied') {
      this.showPushInstructions(
        'Notifications are blocked',
        'Allow notifications for Game Manager in your browser or device settings, then try again.',
      );
      return;
    }

    try {
      this.pushEnabled = await this.pushSubscription.requestPermissionAndSubscribe();
      this.pushPermission = this.pushSubscription.permission();
    } catch (error) {
      console.error('Could not enable turn notifications.', error);
      this.snackBar.open('Could not enable turn notifications. Please try again.', 'Dismiss', {
        duration: 5000,
      });
    }
  }

  showIosInstallInstructions(): void {
    this.showPushInstructions(
      'Add Game Manager to your Home Screen',
      "To get turn notifications on iPhone or iPad, tap the Share button and choose 'Add to Home Screen', then open Game Manager from your Home Screen.",
    );
  }

  async installApp(): Promise<void> {
    try {
      await this.installPrompt.promptInstall();
    } catch (error) {
      console.error('Could not start app installation.', error);
    }
  }

  private async refreshPushStatus(): Promise<void> {
    if (!this.pushSupported) return;
    this.pushEnabled = await this.pushSubscription.isSubscribed();
    this.pushPermission = this.pushSubscription.permission();
  }

  private showPushInstructions(title: string, message: string): void {
    this.dialog.open(PushInstructionsDialogComponent, {
      data: { title, message },
      width: '360px',
    });
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
