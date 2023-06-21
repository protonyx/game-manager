import { Component, Inject } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Player, Tracker } from '../../models/models';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogModule,
} from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { NgIf, NgFor } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-player-edit',
  templateUrl: './player-edit.component.html',
  styleUrls: ['./player-edit.component.scss'],
  standalone: true,
  imports: [
    MatDialogModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    NgIf,
    NgFor,
    MatButtonModule,
  ],
})
export class PlayerEditComponent {
  public player: Player | null | undefined;

  public trackers: Tracker[] | null | undefined;

  playerForm = this.fb.group({
    name: ['', Validators.required],
    trackers: this.fb.group({}),
  });

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<PlayerEditComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.player = data.player;
    this.trackers = data.trackers;

    this.updateTrackers(this.trackers);
    this.playerForm.reset({
      name: this.player!.name,
      trackers: this.player!.trackerValues,
    });
  }

  createTrackerFormGroup(trackers: Tracker[]): FormGroup {
    const group: any = {};

    trackers.forEach((t) => {
      group[t.id] = new FormControl('', Validators.pattern(/-?[0-9]*/));
    });

    return new FormGroup(group);
  }

  updateTrackers(trackers: Tracker[] | null | undefined): void {
    if (!trackers) {
      return;
    }

    const trackerGroup = this.createTrackerFormGroup(trackers);

    this.playerForm.setControl('trackers', trackerGroup);
  }

  handleUpdate(): void {
    this.dialogRef.close(this.playerForm.value);
  }
}
