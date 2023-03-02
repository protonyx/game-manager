import {Component, EventEmitter, Output} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {NewGame} from "../../models/models";
import {NewGamePageComponent} from "../../pages/new-game-page/new-game-page.component";

@Component({
  selector: 'app-new-game',
  templateUrl: './new-game.component.html',
  styleUrls: ['./new-game.component.scss']
})
export class NewGameComponent {

  @Output()
  submit: EventEmitter<NewGame> = new EventEmitter<NewGame>();

  gameForm = this.fb.group({
    name: ['', Validators.required],
    options: this.fb.group({
      shareOtherPlayerTrackers: [true]
    }),
    trackers: this.fb.array([])
  })

  constructor(private fb: FormBuilder) {

  }

  get trackers(): FormArray {
    return this.gameForm.controls["trackers"] as FormArray;
  }

  get trackerControls(): FormGroup[] {
    return this.trackers.controls as FormGroup[];
  }

  addTracker() {
    const trackerForm = this.fb.group({
      name: ['Score', Validators.required],
      startingValue: [0, Validators.required]
    });

    this.trackers.push(trackerForm);
  }

  deleteTracker(trackerIndex: number) {
    this.trackers.removeAt(trackerIndex);
  }

  onSubmit() {
    this.submit.emit(this.gameForm.value as NewGame)
  }

}
