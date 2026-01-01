import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnChanges,
  SimpleChanges
} from '@angular/core';
import {
  FormControl,
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
  ],
})
export class TrackerEditorComponent implements OnChanges {
  @Input()
  public trackerValue: number | null | undefined;

  @Output()
  public trackerChange: EventEmitter<number> =
    new EventEmitter<number>();

  trackerValueControl = new FormControl(0);

  keypadActive = false;
  keypadValue = '';
  isAdding = true;

  public onPlusClick() {
    this.keypadActive = true;
    this.isAdding = true;
    this.keypadValue = '';
  }

  public onMinusClick() {
    this.keypadActive = true;
    this.isAdding = false;
    this.keypadValue = '';
  }

  public onKeypadClick(key: string) {
    if (key === 'backspace') {
      this.keypadValue = this.keypadValue.slice(0, -1);
    } else {
      this.keypadValue += key;
    }
  }

  public onSave() {
    if (this.keypadValue) {
      const delta = parseInt(this.keypadValue) * (this.isAdding ? 1 : -1);
      this.updateTracker(delta);
    }
    this.keypadActive = false;
  }

  public onCancel() {
    this.keypadActive = false;
  }

  public updateTracker(delta: number) {
    let val = this.trackerValueControl.value!;
    val += delta;

    this.trackerValueControl.setValue(val);

    this.trackerChange.emit(val);
  }

  public setTrackerValue(value: number) {
    this.trackerValueControl.setValue(value);

    this.trackerChange.emit(value);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['trackerValue'] && this.trackerValue) {
      this.trackerValueControl.setValue(this.trackerValue);
    }
  }
}
