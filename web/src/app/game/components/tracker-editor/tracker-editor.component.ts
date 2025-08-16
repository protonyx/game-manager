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
    ]
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
    let val = this.player!.trackerValues[trackerId];
    val += delta;

    const control = this.trackerForm.controls[trackerId];
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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['trackers'] && this.trackers) {
      this.trackerForm = this.fb.group({});
      for (const tracker of this.trackers) {
        const startingValue = this.player?.trackerValues[tracker.id] || tracker.startingValue;
        this.trackerForm.addControl(
          tracker.id,
          this.fb.control(startingValue),
        );
      }
    }

    if (changes['player'] && this.player && this.trackers) {
      for (const tracker of this.trackers) {
        const control = this.trackerForm.controls[tracker.id];
        if (control) {
          control.setValue(this.player.trackerValues[tracker.id]);
        }
      }
    }
  }
}
