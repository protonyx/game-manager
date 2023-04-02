import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {Game, Player, Tracker} from "../../models/models";
import {MatTableDataSource, MatTableDataSourcePaginator} from "@angular/material/table";
import {CdkDragDrop} from "@angular/cdk/drag-drop";

@Component({
    selector: 'app-player-list',
    templateUrl: './player-list.component.html',
    styleUrls: ['./player-list.component.scss']
})
export class PlayerListComponent implements OnChanges {
    @Input()
    public game: Game | null = null;

    @Input()
    public players: Player[] | null = null;

    @Input()
    public currentPlayer: Player | null = null;

    @Input()
    public isAdmin: boolean = false;

    @Output()
    public playerOrderUpdated: EventEmitter<Player> = new EventEmitter<Player>();

    @Output()
    public playerNameUpdated: EventEmitter<Player> = new EventEmitter<Player>();

    dataSource: MatTableDataSource<Player> = new MatTableDataSource();

    columnsToDisplay = ['order', 'name'];

    get trackers(): Tracker[] {
        return this.game?.trackers || [];
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ((changes['game'] || changes['players']) && !!this.game && !!this.players && !!this.currentPlayer) {
            this.dataSource = new MatTableDataSource<Player>(this.players)

            this.columnsToDisplay = [];

            if (this.isAdmin) {
                this.columnsToDisplay.push('position')
            }

            this.columnsToDisplay.push('order', 'name');
            for (const tracker of this.game.trackers) {
                this.columnsToDisplay.push(tracker.id);
            }
        }
    }

    checkIsMe(player: Player): boolean {
        return player.id === this.currentPlayer?.id;
    }

    getTrackerValue(player: Player, trackerId: string): string {
        if (trackerId && Object.hasOwn(player.trackerValues, trackerId)) {
            return player.trackerValues[trackerId] as string;
        } else {
            return '???';
        }
    }

    checkIsPlayerTurn(player: Player): boolean {
        return player.id === this.game?.currentTurnPlayerId;
    }

    dropTable(event: CdkDragDrop<MatTableDataSource<Player, MatTableDataSourcePaginator>>): void {
        console.log(event);
        const player = this.players?.find((p) => p === event.item.data);
        const newIndex = event.currentIndex;
        const newPlayer = {...player, order: newIndex + 1}
        // @ts-ignore
        this.playerOrderUpdated.emit(newPlayer);
    }
}
