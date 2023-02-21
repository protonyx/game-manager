import { Component } from '@angular/core';
import {JoinGame} from "../../models/models";
import {GameService} from "../../services/game.service";

@Component({
  selector: 'app-join-game-page',
  templateUrl: './join-game-page.component.html',
  styleUrls: ['./join-game-page.component.scss']
})
export class JoinGamePageComponent {

  constructor(private gameService: GameService) {

  }

  public onJoinGame(event: JoinGame): void {
    console.log(event);

    this.gameService.joinGame(event)
        .subscribe(data => {
          console.log(data);
        })
  }
}
