import {
  Component,
  Inject,
  Input,
  OnChanges,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import {
  CdkDrag,
  CdkDragDrop,
  CdkDragHandle,
  CdkDropList,
  moveItemInArray,
} from '@angular/cdk/drag-drop';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { Player } from '../../models/models';

@Component({
  selector: 'app-player-reorder-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    CdkDrag,
    CdkDragHandle,
    CdkDropList,
  ],
  templateUrl: './player-reorder-modal.component.html',
  styleUrls: ['./player-reorder-modal.component.scss'],
})
export class PlayerReorderModalComponent {
  // @ts-ignore
  @ViewChild('table', { static: true }) table: MatTable<Player>;

  players: Player[] = [];

  columnsToDisplay = ['position', 'order', 'name'];
  dragDisabled = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
    this.players = data.players.map((player: Player) => <Player>{ ...player });
  }

  dropTable(event: CdkDragDrop<Player[]>): void {
    this.dragDisabled = true;

    moveItemInArray(this.players, event.previousIndex, event.currentIndex);
    for (let i = 0; i < this.players.length; i++) {
      this.players[i].order = i + 1;
    }

    this.table.renderRows();
  }
}
