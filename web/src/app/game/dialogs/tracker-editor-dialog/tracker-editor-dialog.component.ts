import { Component, inject } from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {MatButton} from "@angular/material/button";
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import { TrackerEditorComponent } from '../../components/tracker-editor/tracker-editor.component';
import { Store } from '@ngrx/store';
import { selectGameTrackers, selectPlayerById } from '../../state/game.selectors';
import { filter, map } from 'rxjs';
import { Player } from '../../models/models';

@Component({
  selector: 'app-tracker-editor-dialog',
  imports: [
    AsyncPipe,
    MatButton,
    MatDialogModule,
    TrackerEditorComponent,
  ],
  templateUrl: './tracker-editor-dialog.component.html',
  styleUrl: './tracker-editor-dialog.component.scss',
})
export class TrackerEditorDialogComponent {
  private store = inject(Store);

  private readonly data: { playerId: string; trackerId: string } =
    inject(MAT_DIALOG_DATA);

  trackers$ = this.store.select(selectGameTrackers);

  tracker$ = this.trackers$.pipe(
    filter((t) => t !== null && t !== undefined),
    map((trackers) => trackers.find((t) => t.id === this.data.trackerId)),
  );

  trackerName$ = this.tracker$.pipe(map((t) => t?.name));

  player$ = this.store.select(selectPlayerById(this.data.playerId));

  trackerValue$ = this.player$.pipe(
    filter((p) => p !== null && p !== undefined),
    map((p: Player) => p.trackerValues[this.data.trackerId]),
  );

  newTrackerValue = 0;

  private dialogRef = inject(MatDialogRef<TrackerEditorDialogComponent>);

  handleTrackerUpdate(newValue: number) {
    this.newTrackerValue = newValue;
  }
}
