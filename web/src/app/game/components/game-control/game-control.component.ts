import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Game } from '../../models/models';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-game-control',
    templateUrl: './game-control.component.html',
    styleUrls: ['./game-control.component.scss'],
    imports: [CommonModule, MatButtonModule, MatExpansionModule]
})
export class GameControlComponent {
  @Input()
  public game: Game | null = null;

  @Input()
  public isHost: boolean | null = false;

  @Output()
  public startGame: EventEmitter<void> = new EventEmitter<void>();

  @Output()
  public advanceTurn: EventEmitter<void> = new EventEmitter<void>();

  @Output()
  public endGame: EventEmitter<void> = new EventEmitter<void>();

  onStartGame(): void {
    this.startGame.emit();
  }

  onAdvanceTurn(): void {
    this.advanceTurn.emit();
  }

  onEndGame() {
    this.endGame.emit();
  }
}
