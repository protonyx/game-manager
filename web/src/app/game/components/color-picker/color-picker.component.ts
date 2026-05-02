import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PLAYER_COLORS } from '../../models/player-colors';

@Component({
  selector: 'app-color-picker',
  templateUrl: './color-picker.component.html',
  styleUrls: ['./color-picker.component.scss'],
  imports: [CommonModule, MatIconModule, MatTooltipModule],
})
export class ColorPickerComponent {
  @Input() label: string = 'Color';
  @Input() selectedColor: string = '';
  @Input() takenColors: string[] = [];

  @Output() colorSelected = new EventEmitter<string>();

  readonly colors = PLAYER_COLORS;

  isSelected(hex: string): boolean {
    return this.selectedColor?.toLowerCase() === hex.toLowerCase();
  }

  isTaken(hex: string): boolean {
    return this.takenColors.some((c) => c.toLowerCase() === hex.toLowerCase());
  }

  select(hex: string): void {
    if (!this.isTaken(hex)) {
      this.colorSelected.emit(hex);
    }
  }
}
