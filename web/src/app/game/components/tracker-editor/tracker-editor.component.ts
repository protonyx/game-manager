import {Component, EventEmitter, Input, Output, OnChanges, SimpleChanges} from '@angular/core';
import {Player, Tracker, TrackerValue} from "../../models/models";
import { FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { NgFor } from '@angular/common';

@Component({
    selector: 'app-tracker-editor',
    templateUrl: './tracker-editor.component.html',
    styleUrls: ['./tracker-editor.component.scss'],
    standalone: true,
    imports: [FormsModule, ReactiveFormsModule, NgFor, MatButtonModule, MatFormFieldModule, MatInputModule]
})
export class TrackerEditorComponent implements OnChanges {
    @Input()
    public trackers: Tracker[] | null | undefined;

    @Input()
    public player: Player | null | undefined;

    @Output()
    public updateTrackers: EventEmitter<TrackerValue> = new EventEmitter<TrackerValue>();

    trackerValues: TrackerValue[] = [];

    trackerForm: FormGroup = this.fb.group({});

    constructor(private fb: FormBuilder) {

    }

    public updateTracker(trackerId: string, delta: number) {
        const control = this.trackerForm.controls[trackerId]

        let val = control.value
        //let val = this.player?.trackerValues[trackerId];
        val += delta;
        control.setValue(val)

        this.updateTrackers.emit({
            trackerId: trackerId,
            value: val
        })
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['player'] || changes['trackers']) {
            if (this.player && this.trackers) {
                this.trackerForm = this.fb.group({});

                for (const tracker of this.trackers) {
                    this.trackerForm.addControl(tracker.id, this.fb.control(this.player.trackerValues[tracker.id]));
                }
            }
        }
    }
}
