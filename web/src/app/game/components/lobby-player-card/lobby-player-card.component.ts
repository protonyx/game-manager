import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Player } from '../../models/models';

@Component({
  selector: 'app-lobby-player-card',
  templateUrl: './lobby-player-card.component.html',
  styleUrls: ['./lobby-player-card.component.scss'],
  imports: [CommonModule, MatIconModule],
})
export class LobbyPlayerCardComponent {
  @Input() player!: Player;
  @Input() isHost = false;

  get avatarInitial(): string {
    return this.player?.name?.charAt(0).toUpperCase() || '?';
  }
}
