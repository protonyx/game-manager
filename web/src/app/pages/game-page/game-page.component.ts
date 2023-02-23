import {Component, OnInit} from '@angular/core';
import {GameService} from "../../services/game.service";
import {Store} from "@ngrx/store";
import {selectCredentials, selectGame, selectPlayers} from "../../state/games.selectors";
import {SignalrService} from "../../services/signalr.service";
import {GamesApiActions} from "../../state/games.actions";

@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss']
})
export class GamePageComponent implements OnInit {

  credentials$ = this.store.select(selectCredentials)

  game$ = this.store.select(selectGame)

  players$ = this.store.select(selectPlayers)

  constructor(
      private gameService: GameService,
      private signalr: SignalrService,
      private store: Store) {

  }

  ngOnInit(): void {
    this.credentials$.subscribe(data => {
      this.signalr.connect(data!.gameId, data!.token);

      this.gameService.getGame(data!.gameId, data!.token).subscribe(game => {
        this.store.dispatch(GamesApiActions.retrievedGameState({game: game}));
      })
      this.gameService.getPlayers(data!.gameId, data!.token).subscribe(players => {
        this.store.dispatch(GamesApiActions.retrievedPlayers({players: players}));
      })
    })
  }

}
