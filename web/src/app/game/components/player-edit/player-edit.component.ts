import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Player, Tracker } from '../../models/models';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-player-edit',
  templateUrl: './player-edit.component.html',
  styleUrls: ['./player-edit.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
})
export class PlayerEditComponent implements OnChanges {
  @Input()
  public player: Player | null | undefined;

  @Input()
  public trackers: Tracker[] | null | undefined;

  playerForm = this.fb.group({
    name: ['', Validators.required],
    trackers: this.fb.group({}),
  });

  get value(): Player {
    return <Player>this.playerForm.value;
  }

  constructor(
    private fb: FormBuilder,
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
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


}
