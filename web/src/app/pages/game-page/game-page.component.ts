import {Component, OnDestroy, OnInit} from '@angular/core';
import {GameService} from "../../services/game.service";
import {Store} from "@ngrx/store";
import {Subject, Subscription, takeUntil, tap, timer} from "rxjs";
import {selectCredentials, selectGame, selectPlayers} from "../../state/game.reducer";
import {SignalrService} from "../../services/signalr.service";
import {GamesApiActions} from "../../state/game.actions";
import {PlayerCredentials} from "../../models/models";

@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss']
})
export class GamePageComponent implements OnInit, OnDestroy {

  credentials$ = this.store.select(selectCredentials)

  game$ = this.store.select(selectGame)

  players$ = this.store.select(selectPlayers)

  heartbeat$: Subscription | undefined;

  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  constructor(
      private gameService: GameService,
      private signalr: SignalrService,
      private store: Store) {

  }

  ngOnInit(): void {
    this.credentials$.subscribe(credentials => {
      if (credentials) {
        this.gameService.getGame(credentials!.gameId, credentials!.token).subscribe(game => {
          this.store.dispatch(GamesApiActions.retrievedGame({game: game}));
          this.connect(credentials);
        })
        this.gameService.getPlayers(credentials!.gameId, credentials!.token).subscribe(players => {
          this.store.dispatch(GamesApiActions.retrievedPlayers({players: players}));
        })
      }
    })
  }

  ngOnDestroy() {
    this.unsubscribe$.next(true);
    this.unsubscribe$.unsubscribe();
  }

  private connect(credentials: PlayerCredentials) {
    this.signalr.connect(credentials!.gameId, credentials!.token);

    this.heartbeat$ = timer(5000, 30000).pipe(
        takeUntil(this.unsubscribe$)
    ).subscribe(e => {
      this.signalr.heartbeat()
    })
  }

}
