import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {Game, Player, Tracker} from "../../models/models";
import {MatTableDataSource} from "@angular/material/table";

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

    dataSource: MatTableDataSource<Player> = new MatTableDataSource();

    columnsToDisplay = ['name'];

    get trackers(): Tracker[] {
        return this.game?.trackers || [];
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ((changes['game'] || changes['players']) && !!this.game && !!this.players && !!this.currentPlayer) {
            this.dataSource = new MatTableDataSource<Player>(this.players)

            this.columnsToDisplay = ['name'];
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
}
