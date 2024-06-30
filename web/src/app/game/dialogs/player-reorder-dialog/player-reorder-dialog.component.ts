import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import {
  CdkDragDrop,
  DragDropModule,
  moveItemInArray,
} from '@angular/cdk/drag-drop';
import { Player } from '../../models/models';
import { Store } from '@ngrx/store';
import { selectAllPlayers } from '../../state/game.selectors';
import { Subject, takeUntil, tap } from 'rxjs';

@Component({
  selector: 'app-player-reorder-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    DragDropModule,
  ],
  templateUrl: './player-reorder-dialog.component.html',
  styleUrl: './player-reorder-dialog.component.scss',
})
export class PlayerReorderDialogComponent implements OnDestroy {
  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  players$ = this.store
    .select(selectAllPlayers)
    .pipe(
      tap(
        (players) => (this.newPlayers = players.map((p) => <Player>{ ...p })),
      ),
    );

  newPlayers: Player[] = [];

  columnsToDisplay = ['position', 'name'];
  dragDisabled = true;

  constructor(private store: Store) {
    this.players$.pipe(takeUntil(this.unsubscribe$)).subscribe();
  }

  ngOnDestroy() {
    this.unsubscribe$.next(true);
    this.unsubscribe$.unsubscribe();
  }

  dropTable(event: CdkDragDrop<Player[]>, table: MatTable<Player>): void {
    this.dragDisabled = true;

    moveItemInArray(this.newPlayers, event.previousIndex, event.currentIndex);
    for (let i = 0; i < this.newPlayers.length; i++) {
      this.newPlayers[i].order = i + 1;
    }

    table.renderRows();
  }
}
