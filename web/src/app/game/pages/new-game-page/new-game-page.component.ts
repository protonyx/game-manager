import { Component } from '@angular/core';
import {Router} from "@angular/router";
import {JoinGame, NewGame} from "../../models/models";
import {GameService} from "../../services/game.service";
import {Store} from "@ngrx/store";
import {GamesApiActions} from "../../state/game.actions";
import { NewGameComponent } from '../../components/new-game/new-game.component';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-new-game-page',
    templateUrl: './new-game-page.component.html',
    styleUrls: ['./new-game-page.component.scss'],
    standalone: true,
    imports: [MatCardModule, NewGameComponent]
})
export class NewGamePageComponent {

  constructor(
      private router: Router,
      private gameService: GameService,
      private store: Store
  ) {}

  onCreateGame(game: NewGame) {
    this.gameService.createGame(game).subscribe(data => {
      this.store.dispatch(GamesApiActions.createdGame({game: data}));

      const joinGame: JoinGame = {
        entryCode: data.entryCode,
        name: 'Player 1'
      };

      this.gameService.joinGame(joinGame)
            .subscribe(data2 => {
                this.store.dispatch(GamesApiActions.joinedGame({credentials: data2}));
                this.router.navigate(['game'])
            })
    })
  }

}
