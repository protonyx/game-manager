import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Game, Player } from '../../models/models';
import { PatchOperation } from '../../models/patch';
import { ColorPickerComponent } from '../color-picker/color-picker.component';

@Component({
  selector: 'app-player-waiting',
  templateUrl: './player-waiting.component.html',
  styleUrls: ['./player-waiting.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    ColorPickerComponent,
  ],
})
export class PlayerWaitingComponent implements OnChanges {
  @Input() game: Game | null = null;
  @Input() currentPlayer: Player | null = null;
  @Input() takenColors: string[] = [];

  @Output() readyToggled = new EventEmitter<boolean>();
  @Output() playerPatched = new EventEmitter<{ playerId: string; ops: PatchOperation[] }>();

  nameControl = new FormControl('');

  ngOnChanges(): void {
    if (this.currentPlayer) {
      // Only update if the control is pristine or the value differs from external state
      const externalName = this.currentPlayer.name;
      if (this.nameControl.value !== externalName && !this.nameControl.dirty) {
        this.nameControl.setValue(externalName, { emitEvent: false });
      }
    }
  }

  onNameBlur(): void {
    const name = this.nameControl.value?.trim();
    if (!name || !this.currentPlayer) return;
    if (name !== this.currentPlayer.name) {
      this.playerPatched.emit({
        playerId: this.currentPlayer.id,
        ops: [{ op: 'replace', path: '/name', value: name }],
      });
    }
    this.nameControl.markAsPristine();
  }

  onColorSelect(hex: string): void {
    if (!this.currentPlayer) return;
    if (hex !== this.currentPlayer.color) {
      this.playerPatched.emit({
        playerId: this.currentPlayer.id,
        ops: [{ op: 'replace', path: '/color', value: hex }],
      });
    }
  }

  onReadyToggle(): void {
    this.readyToggled.emit(!this.currentPlayer?.isReady);
  }
}
