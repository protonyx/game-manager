import {Injectable} from "@angular/core";
import {Actions, concatLatestFrom, createEffect, ofType} from "@ngrx/effects";
import {GameService} from "../services/game.service";
import {GameActions, GamesApiActions, PlayersApiActions} from "./game.actions";
import {exhaustMap, map, tap} from "rxjs";
import {Store} from "@ngrx/store";
import * as fromGames from "./game.reducer";
import {Router} from "@angular/router";

@Injectable()
export class GameEffects {
    $leaveGame = createEffect(() => this.actions$.pipe(
        ofType(GameActions.leaveGame),
        concatLatestFrom(action => this.store.select(fromGames.selectCurrentPlayer)),
        exhaustMap(([action, player]) => this.gameService.removePlayer(player!.id)
            .pipe(
                tap(() => console.log(player)),
                map(() => PlayersApiActions.playerRemoved({playerId: player!.id}))
            ))
    ));

    $authenticationError = createEffect(() => this.actions$.pipe(
        ofType(GamesApiActions.authenticationError),
        map(() => {
            this.router.navigate(['game', 'join'])

            return GameActions.clearCredentials();
        })
    ))

    constructor(
        private actions$: Actions,
        private store: Store,
        private gameService: GameService,
        private router: Router
    ) {
    }
}