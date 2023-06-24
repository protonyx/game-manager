import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Game } from '../../models/models';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-game-control',
  templateUrl: './game-control.component.html',
  styleUrls: ['./game-control.component.scss'],
  standalone: true,
  imports: [MatButtonModule, MatExpansionModule, NgIf],
})
export class GameControlComponent {
  @Input()
  public game: Game | null = null;

  @Output()
  public startGame: EventEmitter<void> = new EventEmitter<void>();

  onStartGame(): void {
    this.startGame.emit();
  }
}
