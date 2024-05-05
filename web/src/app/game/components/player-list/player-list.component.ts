import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { Game, Player, Tracker } from '../../models/models';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-player-list',
  templateUrl: './player-list.component.html',
  styleUrls: ['./player-list.component.scss'],
  standalone: true,
  imports: [CommonModule, MatTableModule, MatIconModule],
})
export class PlayerListComponent implements OnChanges {
  @Input()
  public game: Game | null = null;

  @Input()
  public players: Player[] | null = null;

  @Input()
  public currentPlayer: Player | null = null;

  @Input()
  public isHost: boolean | null = false;

  @Output()
  public editPlayer: EventEmitter<Player> = new EventEmitter<Player>();

  dataSource: MatTableDataSource<Player> = new MatTableDataSource();

  columnsToDisplay = ['order', 'name'];

  get trackers(): Tracker[] {
    return this.game?.trackers || [];
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      (changes['game'] || changes['players']) &&
      !!this.game &&
      !!this.players
    ) {
      this.dataSource = new MatTableDataSource<Player>(this.players);

      this.columnsToDisplay = [];

      if (this.isHost) {
        this.columnsToDisplay.push('actions');
      }

      this.columnsToDisplay.push('order', 'name');
      for (const tracker of this.game.trackers) {
        this.columnsToDisplay.push(tracker.id);
      }
    }
  }

  checkIsMe(player: Player): boolean {
    return this.currentPlayer != null
      ? player.id === this.currentPlayer?.id
      : false;
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

  handleEditPlayer(player: Player): void {
    this.editPlayer.emit(player);
  }
}
