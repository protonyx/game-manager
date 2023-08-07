import { Injectable } from '@angular/core';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { GameService } from '../services/game.service';
import {
  GameActions,
  GameHubActions,
  GamesApiActions,
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
} from 'rxjs';
import { Store } from '@ngrx/store';
import * as fromGames from './game.reducer';
import { Router } from '@angular/router';
import { LayoutActions } from '../../shared/state/layout.actions';
import { JoinGame } from '../models/models';

@Injectable()
export class GameEffects {
  $createGame = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.createGame),
      exhaustMap((action) =>
        this.gameService.createGame(action.game).pipe(
          map((game) => GamesApiActions.createdGame({ game })),
          catchError((error) => {
            if (error.status === 400) {
              return of(
                GamesApiActions.createdGameError({ error: error.error.title })
              );
            }

            return EMPTY;
          })
        )
      )
    )
  );

  $createdGame = createEffect(() =>
    this.actions$.pipe(
      ofType(GamesApiActions.createdGame),
      exhaustMap((action) => {
        const joinGame: JoinGame = {
          entryCode: action.game.entryCode,
          name: 'Player 1',
        };

        return of(GameActions.joinGame({ joinGame }));
      })
    )
  );

  $joinGame = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.joinGame),
      exhaustMap((action) =>
        this.gameService.joinGame(action.joinGame).pipe(
          map((data) =>
            GamesApiActions.joinedGame({
              credentials: data,
              entryCode: action.joinGame.entryCode,
            })
          ),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                GamesApiActions.joinedGameError({ error: error.error.title })
              );
            }
            return EMPTY;
          })
        )
      )
    )
  );

  $joinedGame = createEffect(
    () =>
      this.actions$.pipe(
        ofType(GamesApiActions.joinedGame),
        tap((action) => {
          this.router.navigate(['game']);
        })
      ),
    { dispatch: false }
  );

  $leaveGame = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.leaveGame),
      concatLatestFrom((action) =>
        this.store.select(fromGames.selectCurrentPlayer)
      ),
      exhaustMap(([action, player]) =>
        this.gameService
          .removePlayer(player!.id)
          .pipe(map(() => GamesApiActions.leftGame()))
      )
    )
  );

  $clearCredentialsOnLeave = createEffect(() =>
    this.actions$.pipe(
      ofType(GamesApiActions.leftGame),
      map(() => GameActions.clearCredentials())
    )
  );

  $loadGame = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.loadGame),
      exhaustMap((action) =>
        this.gameService.getGame(action.gameId).pipe(
          map((game) => GamesApiActions.retrievedGame({ game })),
          catchError((error) => {
            if (error.status === 400) {
              return of(
                GamesApiActions.retrievedGameError({ error: error.error.title })
              );
            }

            return EMPTY;
          })
        )
      )
    )
  );

  $loadPlayers = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.loadPlayers),
      exhaustMap((action) =>
        this.gameService.getPlayers(action.gameId).pipe(
          map((players) => GamesApiActions.retrievedPlayers({ players })),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                GamesApiActions.retrievedPlayersError({
                  error: error.error.title,
                })
              );
            }

            return EMPTY;
          })
        )
      )
    )
  );

  $loadPlayer = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.loadPlayer),
      exhaustMap((action) =>
        this.gameService.getPlayer(action.playerId).pipe(
          map((player) => GamesApiActions.retrievedPlayer({ player })),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                GamesApiActions.retrievedPlayerError({
                  error: error.error.title,
                })
              );
            }

            return EMPTY;
          })
        )
      )
    )
  );

  $removePlayer = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.removePlayer),
      exhaustMap((action) =>
        this.gameService.removePlayer(action.playerId).pipe(
          map(() =>
            PlayersApiActions.playerRemoved({ playerId: action.playerId })
          ),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                PlayersApiActions.playerRemovedError({
                  error: error.error.title,
                })
              );
            }

            return EMPTY;
          })
        )
      )
    )
  );

  $updateTracker = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.updateTracker),
      concatLatestFrom((action) =>
        this.store.select(fromGames.selectCurrentPlayer)
      ),
      exhaustMap(([action, player]) =>
        this.gameService
          .setPlayerTracker(
            player!.id,
            action.tracker.trackerId,
            action.tracker.value
          )
          .pipe(
            map((player) => PlayersApiActions.playerUpdated({ player: player }))
          )
      )
    )
  );

  $updatePlayerOrder = createEffect(
    () =>
      this.actions$.pipe(
        ofType(GameActions.updatePlayerOrder),
        exhaustMap((action) =>
          this.gameService.reorderPlayers(action.gameId, action.players)
        )
      ),
    { dispatch: false }
  );

  $endTurn = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.endTurn),
      exhaustMap((action: { gameId: string }) =>
        this.gameService.endTurn(action.gameId).pipe(
          map(() => GamesApiActions.endTurnSucceeded()),
          catchError((error) => {
            if (error.status == 400) {
              return of(
                GamesApiActions.endTurnError({
                  error: error.error.title,
                })
              );
            }

            return EMPTY;
          })
        )
      )
    )
  );

  startGame$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(GameActions.startGame),
        exhaustMap((action: { gameId: string }) =>
          this.gameService.startGame(action.gameId)
        )
      ),
    { dispatch: false }
  );

  endGame$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(GameActions.endGame),
        exhaustMap((action: { gameId: string }) =>
          this.gameService.endGame(action.gameId)
        )
      ),
    { dispatch: false }
  );

  $authenticationError = createEffect(() =>
    this.actions$.pipe(
      ofType(GamesApiActions.authenticationError),
      tap(() => {
        this.router.navigate(['game', 'join']);
      }),
      exhaustMap((action) => of(GameActions.clearCredentials()))
    )
  );

  $playerKicked = createEffect(() =>
    this.actions$.pipe(
      ofType(GameHubActions.playerLeft),
      concatLatestFrom((action) =>
        this.store.select(fromGames.selectCurrentPlayer)
      ),
      filter(([action, player]) => !!player && player.id === action.playerId),
      mergeMap(([action, player]) => of(GameActions.clearCredentials()))
    )
  );

  $clearCredentials = createEffect(() =>
    this.actions$.pipe(
      ofType(GameActions.clearCredentials),
      tap(() => {
        this.router.navigate(['game', 'join']);
      }),
      exhaustMap(() => of(LayoutActions.resetLayout()))
    )
  );

  constructor(
    private actions$: Actions,
    private store: Store,
    private gameService: GameService,
    private router: Router
  ) {}
}
