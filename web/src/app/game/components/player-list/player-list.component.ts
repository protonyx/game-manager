import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
  ViewChild,
  AfterViewInit,
} from '@angular/core';
import { Game, Player, Tracker } from '../../models/models';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from "@angular/material/menu";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-player-list',
    templateUrl: './player-list.component.html',
    styleUrls: ['./player-list.component.scss'],
    imports: [
        CommonModule,
        MatTableModule,
        MatIconModule,
        MatMenuModule,
        MatButtonModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatPaginatorModule,
        FormsModule
    ]
})
export class PlayerListComponent implements OnChanges, AfterViewInit {
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

  @Output()
  public kickPlayer: EventEmitter<Player> = new EventEmitter<Player>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  dataSource: MatTableDataSource<Player> = new MatTableDataSource();

  // For search functionality
  searchText: string = '';

  // For pagination
  pageSize = 5;
  pageSizeOptions: number[] = [5, 10, 25];

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

      // Set up the filter predicate for search
      this.dataSource.filterPredicate = (data: Player, filter: string) => {
        const searchText = filter.toLowerCase();
        return data.name.toLowerCase().includes(searchText);
      };

      // Apply paginator if it exists
      if (this.paginator) {
        this.dataSource.paginator = this.paginator;
      }

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

  ngAfterViewInit() {
    // Set up the paginator
    if (this.dataSource && this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  // Method to filter players based on search text
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    // Reset to the first page when filtering
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  // Method to get filtered players for the card view
  get filteredPlayers(): Player[] {
    return this.dataSource.filteredData;
  }

  checkIsMe(player: Player): boolean {
    return this.currentPlayer != null
      ? player.id === this.currentPlayer?.id
      : false;
  }

  getTrackerValue(player: Player, trackerId: string): string {
    if (trackerId && Object.hasOwn(player.trackerValues, trackerId)) {
      return player.trackerValues[trackerId].toString();
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

  handleKickPlayer(player: Player): void {
    this.kickPlayer.emit(player);
  }
}
