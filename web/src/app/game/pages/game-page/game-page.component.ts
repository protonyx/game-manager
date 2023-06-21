import {Component, OnDestroy, OnInit} from '@angular/core';
import {Store} from "@ngrx/store";
import {Actions, ofType} from "@ngrx/effects";
import {Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {MatSnackBar, MatSnackBarModule} from "@angular/material/snack-bar";
import {combineLatest, map, Subject, takeUntil, tap} from "rxjs";
import {
    selectCredentials,
    selectCurrentPlayer,
    selectGame,
    selectPlayers
} from "../../state/game.reducer";
import {GameActions, GameHubActions, GamesApiActions} from "../../state/game.actions";
import {LayoutActions} from "../../../shared/state/layout.actions";
import {GameHubService} from "../../services/game-hub.service";
import {GameService} from "../../services/game.service";
import {Player, PlayerCredentials, Tracker, TrackerValue} from "../../models/models";
import {PlayerEditComponent} from "../../components/player-edit/player-edit.component";
import { TrackerEditorComponent } from '../../components/tracker-editor/tracker-editor.component';
import { NgIf, AsyncPipe } from '@angular/common';
import { CurrentTurnComponent } from '../../components/current-turn/current-turn.component';
import { PlayerListComponent } from '../../components/player-list/player-list.component';
import { MatExpansionModule } from '@angular/material/expansion';

@Component({
    selector: 'app-game-page',
    templateUrl: './game-page.component.html',
    styleUrls: ['./game-page.component.scss'],
    standalone: true,
    imports: [MatExpansionModule, PlayerListComponent, CurrentTurnComponent, NgIf, TrackerEditorComponent, AsyncPipe, MatSnackBarModule]
})
export class GamePageComponent implements OnInit, OnDestroy {

    credentials$ = this.store.select(selectCredentials)

    currentPlayer$ = this.store.select(selectCurrentPlayer)

    game$ = this.store.select(selectGame)

    trackers$ = this.game$.pipe(
        map(g => g?.trackers)
    )

    players$ = this.store.select(selectPlayers)

    unsubscribe$: Subject<boolean> = new Subject<boolean>();

    isAdmin: boolean = false;

    trackers: Tracker[] | null | undefined;

    isMyTurn: boolean = false;

    constructor(
        private gameService: GameService,
        private signalr: GameHubService,
        private store: Store,
        private actions$: Actions,
        private router: Router,
        private dialog: MatDialog,
        private snackBar: MatSnackBar) {
        this.game$.pipe(
            tap(g => {
                this.trackers = g?.trackers;
            }),
            takeUntil(this.unsubscribe$)
        ).subscribe();
    }

    ngOnInit(): void {
        this.credentials$.pipe(
            takeUntil(this.unsubscribe$)
        ).subscribe(credentials => {
            if (credentials) {
                this.isAdmin = credentials.isAdmin;

                this.gameService.getGame(credentials!.gameId).subscribe(game => {
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
        });

        combineLatest({
            game: this.game$,
            currentPlayer: this.currentPlayer$
        }).pipe(
            takeUntil(this.unsubscribe$)
        ).subscribe(data => {
            this.isMyTurn = data.game?.currentTurnPlayerId === data.currentPlayer?.id;
        });

        this.actions$.pipe(
            ofType(GameHubActions.hubDisconnected),
            takeUntil(this.unsubscribe$)
        ).subscribe(connected => {
            let snackBarRef = this.snackBar.open("Server Disconnected", "Reconnect")
            snackBarRef.onAction().subscribe(() => {
                this.signalr.reconnect();
            })
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
        this.gameService.setPlayerOrder(player.id, player.order).subscribe(data => {
            // TODO: Update player state in store
        })
    }

    onPlayerEdit(player: Player): void {
        const dialogRef = this.dialog.open(PlayerEditComponent, {
            data: {
                player: player,
                trackers: this.trackers
            }
        });

        dialogRef.afterClosed().subscribe(data => {
            if (data) {
                const ops = [
                    {op: "replace", path: "/name", value: data.name}
                ];
                if (this.trackers && this.trackers.length > 0) {
                    this.trackers.forEach(t => {
                        ops.push({op: "replace", path: `/trackerValues/${t.id}`, value: data.trackers[t.id]})
                    });
                }
                this.gameService.patchPlayer(player.id, ops).subscribe();
            }

        })
    }

    onTrackerUpdate(trackerValue: TrackerValue): void {
        this.store.dispatch(GameActions.updateTracker({tracker: trackerValue}))
    }

    onPlayerKick(player: Player): void {
        this.gameService.removePlayer(player.id).subscribe();
    }

    private connect(credentials: PlayerCredentials) {
        this.signalr.connect(credentials!.gameId, credentials!.token);
    }

}
