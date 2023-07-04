import { Component } from '@angular/core';
import { NewGame } from '../../models/models';
import { Store } from '@ngrx/store';
import { GameActions } from '../../state/game.actions';
import { NewGameComponent } from '../../components/new-game/new-game.component';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-new-game-page',
  templateUrl: './new-game-page.component.html',
  styleUrls: ['./new-game-page.component.scss'],
  standalone: true,
  imports: [CommonModule, MatCardModule, NewGameComponent],
})
export class NewGamePageComponent {
  constructor(private store: Store) {}

  onCreateGame(game: NewGame) {
    this.store.dispatch(GameActions.createGame({ game: game }));
  }
}
