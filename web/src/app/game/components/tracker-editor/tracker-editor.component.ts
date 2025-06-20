import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { Player, Tracker, TrackerValue } from '../../models/models';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatSliderModule } from '@angular/material/slider';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tracker-editor',
  templateUrl: './tracker-editor.component.html',
  styleUrls: ['./tracker-editor.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSliderModule,
    MatIconModule,
    MatCardModule,
  ],
})
export class TrackerEditorComponent implements OnChanges {
  @Input()
  public trackers: Tracker[] | null | undefined;

  @Input()
  public player: Player | null | undefined;

  @Output()
  public updateTrackers: EventEmitter<TrackerValue> =
    new EventEmitter<TrackerValue>();

  trackerForm: FormGroup = this.fb.group({});

  constructor(private fb: FormBuilder) {}

  public updateTracker(trackerId: string, delta: number) {
    const control = this.trackerForm.controls[trackerId];

    let val = control.value;
    val += delta;
    control.setValue(val);

    this.updateTrackers.emit({
      trackerId: trackerId,
      value: val,
    });
  }

  public setTrackerValue(trackerId: string, value: number) {
    const control = this.trackerForm.controls[trackerId];
    control.setValue(value);

    this.updateTrackers.emit({
      trackerId: trackerId,
      value: value,
    });
  }

  // Get min and max values for sliders
  public getMinValue(trackerId: string): number {
    // Default min value, could be customized per tracker in the future
    return 0;
  }

  public getMaxValue(trackerId: string): number {
    // Default max value, could be customized per tracker in the future
    return 100;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['player'] || changes['trackers']) {
      if (this.player && this.trackers) {
        this.trackerForm = this.fb.group({});

        for (const tracker of this.trackers) {
          this.trackerForm.addControl(
            tracker.id,
            this.fb.control(this.player.trackerValues[tracker.id]),
          );
        }
      }
    }
  }
}
