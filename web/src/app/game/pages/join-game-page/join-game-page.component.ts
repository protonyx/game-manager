import {Component, OnDestroy} from '@angular/core';
import { JoinGame } from '../../models/models';
import { GameService } from '../../services/game.service';
import { Store } from '@ngrx/store';
import { GameActions, GamesApiActions } from '../../state/game.actions';
import { Router } from '@angular/router';
import { map, Subject, takeUntil, BehaviorSubject, tap } from 'rxjs';
import { JoinGameComponent } from '../../components/join-game/join-game.component';
import {AsyncPipe} from "@angular/common";
import {Actions, ofType} from "@ngrx/effects";

@Component({
  selector: 'app-join-game-page',
  templateUrl: './join-game-page.component.html',
  styleUrls: ['./join-game-page.component.scss'],
  standalone: true,
  imports: [JoinGameComponent, AsyncPipe],
})
export class JoinGamePageComponent implements OnDestroy {
  loading: boolean = false;

  error$: BehaviorSubject<string> = new BehaviorSubject<string>('');

  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  constructor(
    private gameService: GameService,
    private store: Store,
    private router: Router,
    private actions$: Actions
  ) {
      // Route on success
      actions$.pipe(
          takeUntil(this.unsubscribe$),
          ofType(GamesApiActions.joinedGame),
          tap(() => {
              this.loading = false;
          })
      ).subscribe(() => {
          router.navigate(['/game']);
      })
      // Handle error
      actions$.pipe(
          takeUntil(this.unsubscribe$),
          ofType(GamesApiActions.joinedGameError),
          map(action => action.error),
          tap(() => {
              this.loading = false;
          })
      ).subscribe(this.error$);
  }

  ngOnDestroy() {
    this.unsubscribe$.next(true);
    this.unsubscribe$.unsubscribe();
  }

  public onJoinGame(event: JoinGame): void {
    this.loading = true;
    this.error$.next('');

    this.store.dispatch(GameActions.joinGame({ joinGame: event }));
  }
}
