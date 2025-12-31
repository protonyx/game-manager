import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  MatCard,
  MatCardContent,
  MatCardHeader,
  MatCardTitle,
} from '@angular/material/card';
import {ReactiveFormsModule} from "@angular/forms";
import { Player, Tracker, TrackerValue } from '../../models/models';
import { TrackerEditorComponent } from '../tracker-editor/tracker-editor.component';

@Component({
  selector: 'app-tracker-list',
  imports: [
    MatCard,
    MatCardContent,
    MatCardHeader,
    MatCardTitle,
    ReactiveFormsModule,
    TrackerEditorComponent,
  ],
  templateUrl: './tracker-list.component.html',
  styleUrl: './tracker-list.component.scss',
})
export class TrackerListComponent {
  @Input()
  public trackers: Tracker[] | null | undefined;

  @Input()
  public player: Player | null | undefined;

  @Output()
  public updateTrackers: EventEmitter<TrackerValue> =
    new EventEmitter<TrackerValue>();

  public onTrackerChange(trackerId: string, trackerValue: number) {
    this.updateTrackers.emit({ trackerId: trackerId, value: trackerValue });
  }
}
