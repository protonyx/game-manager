import { Component, Input, OnChanges } from '@angular/core';
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
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PLAYER_COLORS } from '../../models/player-colors';

export interface PlayerEditFormValue {
  name: string;
  color: string;
  trackers: Record<string, number>;
}

@Component({
    selector: 'app-player-edit',
    templateUrl: './player-edit.component.html',
    styleUrls: ['./player-edit.component.scss'],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatIconModule,
        MatTooltipModule,
    ]
})
export class PlayerEditComponent implements OnChanges {
  @Input()
  public player: Player | null | undefined;

  @Input()
  public trackers: Tracker[] | null | undefined;

  @Input()
  public takenColors: string[] = [];

  readonly colors = PLAYER_COLORS;

  playerForm = this.fb.group({
    name: ['', Validators.required],
    color: ['', Validators.required],
    trackers: this.fb.group({}),
  });

  get value(): PlayerEditFormValue {
    return this.playerForm.value as PlayerEditFormValue;
  }

  constructor(private fb: FormBuilder) {}

  ngOnChanges(): void {
    this.updateTrackers(this.trackers);
    this.playerForm.reset({
      name: this.player!.name,
      color: this.player!.color ?? '',
      trackers: this.player!.trackerValues,
    });
  }

  selectColor(hex: string): void {
    if (!this.isColorTaken(hex)) {
      this.playerForm.patchValue({ color: hex });
    }
  }

  isColorTaken(hex: string): boolean {
    return this.takenColors.some(c => c.toLowerCase() === hex.toLowerCase());
  }

  isColorSelected(hex: string): boolean {
    return this.playerForm.value.color?.toLowerCase() === hex.toLowerCase();
  }

  createTrackerFormGroup(trackers: Tracker[]): FormGroup {
    const group: Record<string, FormControl> = {};

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
