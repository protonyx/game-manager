import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {Game, Player, Tracker} from "../../models/models";
import { MatTableDataSource, MatTableDataSourcePaginator, MatTableModule } from "@angular/material/table";
import { CdkDragDrop, CdkDropList, CdkDragHandle, CdkDrag } from "@angular/cdk/drag-drop";
import { NgIf, NgFor } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-player-list',
    templateUrl: './player-list.component.html',
    styleUrls: ['./player-list.component.scss'],
    standalone: true,
    imports: [MatTableModule, CdkDropList, MatIconModule, CdkDragHandle, NgIf, NgFor, CdkDrag]
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
    public editPlayer: EventEmitter<Player> = new EventEmitter<Player>();

    @Output()
    public kickPlayer: EventEmitter<Player> = new EventEmitter<Player>();

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
                this.columnsToDisplay.push('position', 'actions')
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

    handleEditPlayer(player: Player): void {
        this.editPlayer.emit(player);
    }

    handleRemovePlayer(player: Player): void {
        this.kickPlayer.emit(player);
    }
}
