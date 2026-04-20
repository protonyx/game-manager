import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { Game, Player } from '../../models/models';
import { LobbyPlayerCardComponent } from '../lobby-player-card/lobby-player-card.component';

@Component({
  selector: 'app-host-lobby',
  templateUrl: './host-lobby.component.html',
  styleUrls: ['./host-lobby.component.scss'],
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatBadgeModule,
    LobbyPlayerCardComponent,
  ],
})
export class HostLobbyComponent {
  @Input() game: Game | null = null;
  @Input() players: Player[] | null = null;
  @Input() currentPlayer: Player | null = null;

  @Output() startGame = new EventEmitter<void>();

  get nonHostPlayers(): Player[] {
    return (this.players || []).filter(
      (p) => p.id !== this.currentPlayer?.id,
    );
  }

  get allReady(): boolean {
    return (
      this.nonHostPlayers.length > 0 &&
      this.nonHostPlayers.every((p) => p.isReady === true)
    );
  }

  get playerCount(): number {
    return this.players?.length ?? 0;
  }

  onStartGame(): void {
    this.startGame.emit();
  }

  onStartAnyways(): void {
    if (window.confirm('Start the game now, even though not all players are ready?')) {
      this.startGame.emit();
    }
  }
}
