import {Component} from '@angular/core';
import {JoinGame} from "../../models/models";
import {GameService} from "../../services/game.service";
import {Store} from "@ngrx/store";
import {GameActions, GamesApiActions} from "../../state/game.actions";
import {Router} from "@angular/router";

@Component({
    selector: 'app-join-game-page',
    templateUrl: './join-game-page.component.html',
    styleUrls: ['./join-game-page.component.scss']
})
export class JoinGamePageComponent {

    constructor(
        private gameService: GameService,
        private store: Store,
        private router: Router) {

    }

    public onJoinGame(event: JoinGame): void {
        this.store.dispatch(GameActions.joinGame({ joinGame: event }));

        this.gameService.joinGame(event)
            .subscribe(data => {
                this.store.dispatch(GamesApiActions.joinedGame({credentials: data}));
                this.router.navigate(['game'])
            })
    }
}
