import { Component, OnDestroy, OnInit } from '@angular/core';
import { JoinGame } from '../../models/models';
import { Store } from '@ngrx/store';
import { GameActions, GamesApiActions } from '../../state/game.actions';
import { map, Subject, takeUntil, BehaviorSubject, tap } from 'rxjs';
import { JoinGameComponent } from '../../components/join-game/join-game.component';
import { SessionListComponent } from '../../components/session-list/session-list.component';
import { CommonModule } from '@angular/common';
import { Actions, ofType } from '@ngrx/effects';
import { MatTabsModule } from '@angular/material/tabs';
import { SessionService, StoredSession } from '../../services/session.service';

@Component({
    selector: 'app-join-game-page',
    templateUrl: './join-game-page.component.html',
    styleUrls: ['./join-game-page.component.scss'],
    imports: [CommonModule, JoinGameComponent, SessionListComponent, MatTabsModule]
})
export class JoinGamePageComponent implements OnDestroy, OnInit {
  loading = false;

  error$: BehaviorSubject<string> = new BehaviorSubject<string>('');

  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  sessions: StoredSession[] = [];

  constructor(
    private store: Store,
    private actions$: Actions,
    private sessionService: SessionService,
  ) {
    // Handle error
    actions$
      .pipe(
        takeUntil(this.unsubscribe$),
        ofType(GamesApiActions.joinedGameError),
        map((action) => action.error),
        tap(() => {
          this.loading = false;
        }),
      )
      .subscribe(this.error$);
  }

  ngOnInit(): void {
    this.sessions = this.sessionService.getSessions();
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

  public onRejoinSession(session: StoredSession): void {
    this.store.dispatch(GameActions.restoreSession({ session }));
  }
}
