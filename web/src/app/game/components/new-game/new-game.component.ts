import { Component, EventEmitter, Output } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { NewGame } from '../../models/models';
import { NewGamePageComponent } from '../../pages/new-game-page/new-game-page.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { NgFor } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-new-game',
  templateUrl: './new-game.component.html',
  styleUrls: ['./new-game.component.scss'],
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    NgFor,
    MatButtonModule,
    MatIconModule,
  ],
})
export class NewGameComponent {
  @Output()
  newGame: EventEmitter<NewGame> = new EventEmitter<NewGame>();

  gameForm = this.fb.group({
    name: ['', Validators.required],
    options: this.fb.group({
      shareOtherPlayerTrackers: [true],
    }),
    trackers: this.fb.array([]),
  });

  constructor(private fb: FormBuilder) {}

  get trackers(): FormArray {
    return this.gameForm.controls['trackers'] as FormArray;
  }

  get trackerControls(): FormGroup[] {
    return this.trackers.controls as FormGroup[];
  }

  addTracker() {
    const trackerForm = this.fb.group({
      name: ['Score', Validators.required],
      startingValue: [0, Validators.required],
    });

    this.trackers.push(trackerForm);
  }

  deleteTracker(trackerIndex: number) {
    this.trackers.removeAt(trackerIndex);
  }

  onSubmit() {
    this.newGame.emit(this.gameForm.value as NewGame);
  }
}
