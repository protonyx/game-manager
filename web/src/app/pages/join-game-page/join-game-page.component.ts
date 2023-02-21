import { Component } from '@angular/core';
import {JoinGame} from "../../models/models";
import {GameService} from "../../services/game.service";
import {Store} from "@ngrx/store";
import {GamesActions, GamesApiActions} from "../../state/games.actions";
import {SignalrService} from "../../services/signalr.service";

@Component({
  selector: 'app-join-game-page',
  templateUrl: './join-game-page.component.html',
  styleUrls: ['./join-game-page.component.scss']
})
export class JoinGamePageComponent {

  constructor(
      private gameService: GameService,
      private signalr: SignalrService,
      private store: Store) {

  }

  public onJoinGame(event: JoinGame): void {
    console.log(event);

    this.gameService.joinGame(event)
        .subscribe(data => {
          console.log(data);
          this.store.dispatch(GamesApiActions.joinedGame({ credentials: data }));
          this.signalr.connect(data.gameId, data.token);
        })
  }
}
