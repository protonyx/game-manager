import { Component } from '@angular/core';
import { JoinGame } from '../../models/models';
import { GameService } from '../../services/game.service';
import { Store } from '@ngrx/store';
import { GameActions, GamesApiActions } from '../../state/game.actions';
import { Router } from '@angular/router';
import { catchError } from 'rxjs';
import { JoinGameComponent } from '../../components/join-game/join-game.component';

@Component({
  selector: 'app-join-game-page',
  templateUrl: './join-game-page.component.html',
  styleUrls: ['./join-game-page.component.scss'],
  standalone: true,
  imports: [JoinGameComponent],
})
export class JoinGamePageComponent {
  loading: boolean = false;

  errorMessage: string | undefined;

  constructor(
    private gameService: GameService,
    private store: Store,
    private router: Router
  ) {}

  public onJoinGame(event: JoinGame): void {
    this.loading = true;
    this.errorMessage = '';

    this.store.dispatch(GameActions.joinGame({ joinGame: event }));

    this.gameService
      .joinGame(event)
      .pipe(
        catchError((err, caught) => {
          this.loading = false;

          if (err.status === 400) {
            this.errorMessage = err.error.title;
          }

          throw err;
        })
      )
      .subscribe((data) => {
        this.store.dispatch(GamesApiActions.joinedGame({ credentials: data }));
        this.router.navigate(['game']);
      });
  }
}
