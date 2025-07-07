import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Game, GameState } from '../../models/models';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatCard, MatCardContent, MatCardHeader, MatCardTitle } from '@angular/material/card';

@Component({
  selector: 'app-game-control',
  templateUrl: './game-control.component.html',
  styleUrls: ['./game-control.component.scss'],
  imports: [
    CommonModule,
    MatButtonModule,
    MatExpansionModule,
    MatToolbarModule,
    MatTooltipModule,
    MatMenuModule,
    MatIconModule,
    MatCard,
    MatCardHeader,
    MatCardContent,
    MatCardTitle,
  ],
  standalone: true,
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

  protected readonly GameState = GameState;
}
