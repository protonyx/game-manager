import { Component, Inject } from '@angular/core';
import { Store } from '@ngrx/store';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import {
  selectCurrentPlayerIsHost,
  selectGameTrackers,
  selectPlayerById,
} from '../../state/game.selectors';
import { MatButton } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import {
  PlayerEditComponent,
  PlayerEditFormValue,
} from '../../components/player-edit/player-edit.component';
import { Player, Tracker } from '../../models/models';
import { Observable } from 'rxjs';
import { PatchOperation } from '../../models/patch';

@Component({
    selector: 'app-player-edit-dialog',
    imports: [CommonModule, MatDialogModule, MatButton, PlayerEditComponent],
    templateUrl: './player-edit-dialog.component.html',
    styleUrl: './player-edit-dialog.component.scss'
})
export class PlayerEditDialogComponent {
  isHost$ = this.store.select(selectCurrentPlayerIsHost);

  trackers$ = this.store.select(selectGameTrackers);

  player$: Observable<Player>;

  constructor(
    private store: Store,
    public dialogRef: MatDialogRef<PlayerEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { playerId: string },
  ) {
    this.player$ = this.store.select(
      selectPlayerById(data.playerId),
    ) as Observable<Player>;
  }

  formatOps(
    value: PlayerEditFormValue,
    trackers: Tracker[] | null | undefined,
  ): PatchOperation[] {
    const ops: PatchOperation[] = [
      { op: 'replace', path: '/name', value: value.name },
    ];

    if (trackers && trackers.length > 0) {
      trackers.forEach((t) => {
        ops.push({
          op: 'replace',
          path: `/trackerValues/${t.id}`,
          value: value.trackers[t.id],
        });
      });
    }

    return ops;
  }

  handleUpdate(): void {
    //this.dialogRef.close(this.playerForm.value);
  }
}
