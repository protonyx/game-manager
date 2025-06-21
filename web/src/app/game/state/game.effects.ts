import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { GameService } from '../services/game.service';
import {
  GameActions,
  GameHubActions,
  GamesApiActions,
  PlayerReorderDialogActions,
  PlayersApiActions,
} from './game.actions';
import {
  catchError,
  EMPTY,
  exhaustMap,
  map,
  of,
  mergeMap,
  tap,
  filter,
  concatMap,
} from 'rxjs';
import { Store } from '@ngrx/store';
import * as fromGames from './game.selectors';
import { Router } from '@angular/router';
import { LayoutActions } from '../../shared/state/layout.actions';
import { JoinGame, Player } from '../models/models';
import { routerNavigatedAction, getRouterSelectors } from '@ngrx/router-store';
import { GameHubService } from '../services/game-hub.service';
import { fromPromise } from 'rxjs/internal/observable/innerFrom';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PlayerEditDialogComponent } from '../dialogs/player-edit-dialog/player-edit-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { PlayerReorderDialogComponent } from '../dialogs/player-reorder-dialog/player-reorder-dialog.component';
import { PatchOperation } from '../models/patch';

const { selectRouteParam, selectCurrentRoute } = getRouterSelectors();

const selectIdRouteParam = selectRouteParam('id');

export const createGame = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.createGame),
      exhaustMap((action) =>
        gameService.createGame(action.game).pipe(
          map((game) => GamesApiActions.createdGame({ game })),
          catchError((error) => {
            if (error.status === 400) {
              return of(
                GamesApiActions.createdGameError({ error: error.error.title }),
              );
            }

            return EMPTY;
          }),
        ),
      ),
    );
  },
  { functional: true },
);

export const joinGameOnCreate = createEffect(
  (actions$ = inject(Actions)) => {
    return actions$.pipe(
      ofType(GamesApiActions.createdGame),
      exhaustMap((action) => {
        const joinGame: JoinGame = {
          entryCode: action.game.entryCode,
          name: 'Player 1',
          observer: false,
        };

        return of(GameActions.joinGame({ joinGame }));
      }),
    );
  },
  { functional: true },
);

export const joinGame = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.joinGame),
      exhaustMap((action) =>
        gameService.joinGame(action.joinGame).pipe(
          map((data) =>
            GamesApiActions.joinedGame({
              credentials: data,
              entryCode: action.joinGame.entryCode,
            }),
          ),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                GamesApiActions.joinedGameError({ error: error.error.detail }),
              );
            }
            return EMPTY;
          }),
        ),
      ),
    );
  },
  { functional: true },
);

export const redirectAfterJoinedGame = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) => {
    return actions$.pipe(
      ofType(GamesApiActions.joinedGame),
      tap(() => {
        router.navigate(['game']);
      }),
    );
  },
  { functional: true, dispatch: false },
);

export const leaveGame = createEffect(
  (
    actions$ = inject(Actions),
    store = inject(Store),
    gameService = inject(GameService),
  ) => {
    return actions$.pipe(
      ofType(GameActions.leaveGame),
      concatLatestFrom(() => store.select(fromGames.selectCurrentPlayer)),
      exhaustMap(([, player]) =>
        gameService
          .removePlayer(player!.id)
          .pipe(map(() => GamesApiActions.leftGame())),
      ),
    );
  },
  { functional: true },
);

export const clearCredentialsOnLeave = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) => {
    return actions$.pipe(
      ofType(GamesApiActions.leftGame),
      tap(() => {
        router.navigate(['./join']);
      }),
      map(() => GameActions.clearCredentials()),
    );
  },
  { functional: true },
);

export const loadGameOnJoin = createEffect(
  (actions$ = inject(Actions)) => {
    return actions$.pipe(
      ofType(GamesApiActions.joinedGame),
      map(({ credentials }) =>
        GameActions.loadGame({ gameId: credentials.gameId }),
      ),
    );
  },
  { functional: true },
);

export const loadPlayersOnLoadGame = createEffect(
  (actions$ = inject(Actions)) => {
    return actions$.pipe(
      ofType(GameActions.loadGame),
      map((action) => GameActions.loadPlayers({ gameId: action.gameId })),
    );
  },
  { functional: true },
);

export const loadGame = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.loadGame),
      exhaustMap((action) =>
        gameService.getGame(action.gameId).pipe(
          map((game) => GamesApiActions.retrievedGame({ game })),
          catchError((error) => {
            if (error.status === 400) {
              return of(
                GamesApiActions.retrievedGameError({
                  error: error.error.title,
                }),
              );
            }

            return EMPTY;
          }),
        ),
      ),
    );
  },
  { functional: true },
);

export const editPlayerDialog = createEffect(
  (actions$ = inject(Actions), dialog = inject(MatDialog)) => {
    return actions$.pipe(
      ofType(GameActions.editPlayer),
      exhaustMap((action) => {
        const dialogRef = dialog.open(PlayerEditDialogComponent, {
          data: {
            playerId: action.playerId,
          },
          width: '400px',
        });

        return dialogRef.afterClosed().pipe(
          filter(
            (result: string | PatchOperation[] | undefined) =>
              result !== undefined,
          ),
          map((result: string | PatchOperation[] | undefined) => {
            if (result === 'kick') {
              return GameActions.removePlayer({ playerId: action.playerId });
            }

            return GamesApiActions.patchPlayer({
              playerId: action.playerId,
              ops: result as PatchOperation[],
            });
          }),
        );
      }),
    );
  },
  { functional: true },
);

export const reorderPlayerDialog = createEffect(
  (actions$ = inject(Actions), dialog = inject(MatDialog)) => {
    return actions$.pipe(
      ofType(GameActions.reorderPlayers),
      exhaustMap(() => {
        const dialogRef = dialog.open(PlayerReorderDialogComponent, {
          width: '400px',
        });

        return dialogRef.afterClosed();
      }),
      map((result: undefined | Player[]) => {
        if (result === undefined) {
          return PlayerReorderDialogActions.closed();
        }

        return PlayerReorderDialogActions.updatePlayerOrder({
          players: result,
        });
      }),
    );
  },
  { functional: true },
);

export const patchPlayer = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GamesApiActions.patchPlayer),
      exhaustMap((action) => {
        return gameService.patchPlayer(action.playerId, action.ops).pipe(
          map((player) => GamesApiActions.patchedPlayer({ player: player })),
          catchError((error) => {
            if (error.status === 400) {
              return of(
                GamesApiActions.patchedPlayerError({
                  error: error.error.title,
                }),
              );
            }

            return EMPTY;
          }),
        );
      }),
    );
  },
  { functional: true },
);

export const updateLayoutOnGameLoaded = createEffect(
  (actions$ = inject(Actions)) => {
    return actions$.pipe(
      ofType(GamesApiActions.retrievedGame),
      map(({ game }) =>
        LayoutActions.setHeader({
          title: game.name,
          entryCode: game.entryCode,
        }),
      ),
    );
  },
  { functional: true },
);

export const loadGameSummary = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.loadGameSummary),
      exhaustMap((action) =>
        gameService.getGameSummary(action.gameId).pipe(
          map((summary) => GamesApiActions.retrievedGameSummary({ summary })),
          catchError((error) => {
            if (error.status === 400) {
              return of(
                GamesApiActions.retrievedGameSummaryError({
                  error: error.error.title,
                }),
              );
            }

            return EMPTY;
          }),
        ),
      ),
    );
  },
  { functional: true },
);

export const loadPlayers = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.loadPlayers),
      exhaustMap((action) =>
        gameService.getPlayers(action.gameId).pipe(
          map((players) => GamesApiActions.retrievedPlayers({ players })),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                GamesApiActions.retrievedPlayersError({
                  error: error.error.title,
                }),
              );
            }

            return EMPTY;
          }),
        ),
      ),
    );
  },
  { functional: true },
);

export const loadPlayer = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.loadPlayer),
      exhaustMap((action) =>
        gameService.getPlayer(action.playerId).pipe(
          map((player) => GamesApiActions.retrievedPlayer({ player })),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                GamesApiActions.retrievedPlayerError({
                  error: error.error.title,
                }),
              );
            }

            return EMPTY;
          }),
        ),
      ),
    );
  },
  { functional: true },
);

export const removePlayer = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.removePlayer),
      mergeMap((action) =>
        gameService.removePlayer(action.playerId).pipe(
          map(() =>
            PlayersApiActions.playerRemoved({ playerId: action.playerId }),
          ),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                PlayersApiActions.playerRemovedError({
                  error: error.error.title,
                }),
              );
            }

            return EMPTY;
          }),
        ),
      ),
    );
  },
  { functional: true },
);

export const updateTracker = createEffect(
  (
    actions$ = inject(Actions),
    store = inject(Store),
    gameService = inject(GameService),
  ) => {
    return actions$.pipe(
      ofType(GameActions.updateTracker),
      concatLatestFrom(() => store.select(fromGames.selectCurrentPlayer)),
      concatMap(([action, player]) =>
        gameService
          .setPlayerTracker(
            player!.id,
            action.tracker.trackerId,
            action.tracker.value,
          )
          .pipe(
            map((player) =>
              PlayersApiActions.playerUpdated({ player: player }),
            ),
          ),
      ),
    );
  },
  { functional: true },
);

export const updatePlayerOrder = createEffect(
  (
    actions$ = inject(Actions),
    store = inject(Store),
    gameService = inject(GameService),
  ) => {
    return actions$.pipe(
      ofType(PlayerReorderDialogActions.updatePlayerOrder),
      concatLatestFrom(() => store.select(fromGames.selectCredentials)),
      exhaustMap(([action, credentials]) =>
        gameService.reorderPlayers(credentials!.gameId, action.players),
      ),
    );
  },
  { functional: true, dispatch: false },
);

export const endTurn = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.endTurn),
      exhaustMap((action: { gameId: string }) =>
        gameService.endTurn(action.gameId).pipe(
          map(() => GamesApiActions.endTurnSucceeded()),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                GamesApiActions.endTurnError({
                  error: error.error.title,
                }),
              );
            }

            return EMPTY;
          }),
        ),
      ),
    );
  },
  { functional: true },
);

export const startGame = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.startGame),
      exhaustMap((action: { gameId: string }) =>
        gameService.startGame(action.gameId),
      ),
    );
  },
  { functional: true, dispatch: false },
);

export const endGame = createEffect(
  (actions$ = inject(Actions), gameService = inject(GameService)) => {
    return actions$.pipe(
      ofType(GameActions.endGame),
      exhaustMap((action: { gameId: string }) =>
        gameService.endGame(action.gameId),
      ),
    );
  },
  { functional: true, dispatch: false },
);

export const clearCredentialsOnAuthenticationError = createEffect(
  (actions$ = inject(Actions), router = inject(Router)) => {
    return actions$.pipe(
      ofType(GamesApiActions.authenticationError),
      tap(() => {
        router.navigate(['game', 'join']);
      }),
      map(() => GameActions.clearCredentials()),
    );
  },
  { functional: true },
);

export const clearCredentialsWhenKicked = createEffect(
  (actions$ = inject(Actions), store = inject(Store), router = inject(Router)) => {
    return actions$.pipe(
      ofType(GameHubActions.playerLeft),
      concatLatestFrom(() => store.select(fromGames.selectCurrentPlayerId)),
      filter(([action, playerId]) => action.playerId === playerId),
      tap(() => {
          router.navigate(['game', 'join']);
      }),
      map(() => GameActions.clearCredentials()),
    );
  },
  { functional: true },
);

export const resetLayoutAfterClearCredentials = createEffect(
  (actions$ = inject(Actions)) => {
    return actions$.pipe(
      ofType(GameActions.clearCredentials),
      map(() => LayoutActions.resetLayout()),
    );
  },
  { functional: true },
);

export const gameEnded = createEffect(
  (actions$ = inject(Actions)) => {
    return actions$.pipe(
      ofType(GameHubActions.gameUpdated, GamesApiActions.retrievedGame),
      filter((action) => action.game.state === 'Complete'),
      map((action) => GameHubActions.gameEnded({ gameId: action.game.id })),
    );
  },
  { functional: true },
);

export const clearCredentialsOnGameEnded = createEffect(
  (actions$ = inject(Actions)) => {
    return actions$.pipe(
      ofType(GameHubActions.gameEnded),
      map(() => GameActions.clearCredentials()),
    );
  },
  { functional: true },
);

export const redirectToSummaryOnGameEnd = createEffect(
  (
    actions$ = inject(Actions),
    store = inject(Store),
    router = inject(Router),
  ) => {
    return actions$.pipe(
      ofType(GameHubActions.gameEnded),
      concatLatestFrom(() => store.select(fromGames.selectGame)),
      filter(([action, currentGame]) => action.gameId === currentGame?.id),
      tap(([action]) => {
        router.navigate(['game', 'summary', action.gameId]);
      }),
    );
  },
  { functional: true, dispatch: false },
);

export const loadGameSummaryOnNavigate = createEffect(
  (actions$ = inject(Actions), store = inject(Store)) => {
    return actions$.pipe(
      ofType(routerNavigatedAction),
      concatLatestFrom(() => [
        store.select(selectCurrentRoute),
        store.select(selectIdRouteParam),
      ]),
      filter(([, route]) => route.routeConfig.path === 'summary/:id'),
      map(([, , paramId]) => GameActions.loadGameSummary({ gameId: paramId! })),
    );
  },
  { functional: true },
);

export const loadGameOnNavigateToGamePage = createEffect(
  (actions$ = inject(Actions), store = inject(Store)) => {
    return actions$.pipe(
      ofType(routerNavigatedAction),
      concatLatestFrom(() => [
        store.select(selectCurrentRoute),
        store.select(fromGames.selectCredentials),
      ]),
      filter(
        ([, route, credentials]) =>
          route.routeConfig.title === 'GamePage' && credentials != null,
      ),
      map(([, , credentials]) => {
        return GameActions.loadGame({ gameId: credentials!.gameId });
      }),
    );
  },
  { functional: true },
);

export const signalrConnectOnNavigateToGamePage = createEffect(
  (
    actions$ = inject(Actions),
    store = inject(Store),
    hubService = inject(GameHubService),
  ) => {
    return actions$.pipe(
      ofType(routerNavigatedAction),
      concatLatestFrom(() => [
        store.select(selectCurrentRoute),
        store.select(fromGames.selectCredentials),
      ]),
      filter(
        ([, route, credentials]) =>
          route.routeConfig.title === 'GamePage' && credentials != null,
      ),
      tap(([, , credentials]) => {
        hubService.connect(credentials!.gameId, credentials!.token);
      }),
    );
  },
  { functional: true, dispatch: false },
);

export const signalrDisconnectOnClearCredentials = createEffect(
  (actions$ = inject(Actions), hubService = inject(GameHubService)) => {
    return actions$.pipe(
      ofType(GameActions.clearCredentials),
      tap(() => {
        hubService.disconnect();
      }),
    );
  },
  { functional: true, dispatch: false },
);

export const snackbarOnSignalrDisconnect = createEffect(
  (
    actions$ = inject(Actions),
    store = inject(Store),
    snackbar = inject(MatSnackBar),
  ) => {
    return actions$.pipe(
      ofType(GameHubActions.hubDisconnected),
      concatLatestFrom(() => store.select(fromGames.selectCredentials)),
      filter(([, credentials]) => credentials != null),
      exhaustMap(() => {
        const snackBarRef = snackbar.open('Server Disconnected', 'Reconnect');
        return snackBarRef.onAction();
      }),
      map(() => GameHubActions.hubReconnect()),
    );
  },
  { functional: true },
);

export const snackbarDismissOnSignalrConnect = createEffect(
  (actions$ = inject(Actions), snackbar = inject(MatSnackBar)) => {
    return actions$.pipe(
      ofType(GameHubActions.hubConnected),
      tap(() => {
        snackbar.dismiss();
      }),
    );
  },
  { functional: true, dispatch: false },
);

export const signalrReconnect = createEffect(
  (actions$ = inject(Actions), hubService = inject(GameHubService)) => {
    return actions$.pipe(
      ofType(GameHubActions.hubReconnect),
      exhaustMap(() => {
        return fromPromise(hubService.reconnect());
      }),
    );
  },
  { functional: true, dispatch: false },
);
