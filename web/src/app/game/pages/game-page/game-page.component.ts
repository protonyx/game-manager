import {Component, OnDestroy, OnInit} from '@angular/core';
import {GameService} from "../../services/game.service";
import {Store} from "@ngrx/store";
import {catchError, Subject, Subscription, takeUntil, tap, timer} from "rxjs";
import {selectCredentials, selectCurrentPlayer, selectGame, selectPlayers} from "../../state/game.reducer";
import {GameHubService} from "../../services/game-hub.service";
import {GameActions, GamesApiActions} from "../../state/game.actions";
import {Player, PlayerCredentials} from "../../models/models";
import {LayoutActions} from "../../../shared/state/layout.actions";
import {Router} from "@angular/router";

@Component({
  selector: 'app-game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss']
})
export class GamePageComponent implements OnInit, OnDestroy {

  credentials$ = this.store.select(selectCredentials)

  currentPlayer$ = this.store.select(selectCurrentPlayer)

  game$ = this.store.select(selectGame)

  players$ = this.store.select(selectPlayers)

  heartbeat$: Subscription | undefined;

  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  isAdmin: boolean = false;

  constructor(
      private gameService: GameService,
      private signalr: GameHubService,
      private store: Store,
      private router: Router) {

  }

  ngOnInit(): void {
    this.credentials$.subscribe(credentials => {
      if (credentials) {
        this.isAdmin = credentials.isAdmin;

        this.gameService.getGame(credentials!.gameId).pipe(
            catchError((err, caught) => {
              this.store.dispatch(GameActions.leaveGame())

              throw err;
            })
        ).subscribe(game => {
          this.store.dispatch(GamesApiActions.retrievedGame({game: game}));
          this.store.dispatch(LayoutActions.setTitle({title: game.name}))
          this.connect(credentials);
        })
        this.gameService.getPlayer(credentials!.playerId).subscribe(player => {
          this.store.dispatch(GamesApiActions.retrievedCurrentPlayer({player: player}));
        })
        this.gameService.getPlayers(credentials!.gameId).subscribe(players => {
          this.store.dispatch(GamesApiActions.retrievedPlayers({players: players}));
        })
      }
    })
  }

  ngOnDestroy() {
    this.unsubscribe$.next(true);
    this.unsubscribe$.unsubscribe();

    this.signalr.disconnect();
  }

  onEndTurn(): void {
    this.signalr.endTurn();
  }

  async onLeave(): Promise<void> {
    await this.signalr.disconnect();
    this.store.dispatch(GameActions.leaveGame());

    await this.router.navigate(['./join']);
  }

  onPlayerOrderUpdated(player: Player): void {
    this.gameService.updatePlayer(player.id, player).subscribe(data => {
      // TODO: Update player state in store
    });
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
