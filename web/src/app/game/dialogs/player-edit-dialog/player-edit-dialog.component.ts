import { Component, Inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { selectCurrentPlayerIsHost, selectGameTrackers, selectPlayerById } from '../../state/game.reducer';
import { MatButton } from '@angular/material/button';
import { CommonModule, NgIf } from '@angular/common';
import { PlayerEditComponent } from '../../components/player-edit/player-edit.component';
import { Player, Tracker } from '../../models/models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-player-edit-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButton,
    PlayerEditComponent
  ],
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
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.player$ = this.store.select(selectPlayerById(data.playerId)) as Observable<Player>;
  }

  formatOps(value: any, trackers: Tracker[] | null | undefined): any {
    const ops = [{ op: 'replace', path: '/name', value: value.name }];

    if (trackers && trackers.length > 0) {
      trackers.forEach((t) => {
        ops.push({
          op: 'replace',
          path: `/trackerValues/${t.id}`,
          value: value.trackers[t.id]
        });
      });
    }

    return ops;
  }

  handleUpdate(): void {
    //this.dialogRef.close(this.playerForm.value);
  }
}
