import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { Game, Player } from '../../models/models';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatBadgeModule } from '@angular/material/badge';
import { TurnTimerComponent } from '../turn-timer/turn-timer.component';
import { CommonModule } from '@angular/common';
import { PlayerService } from '../../services/player.service';

@Component({
    selector: 'app-current-turn',
    templateUrl: './current-turn.component.html',
    styleUrls: ['./current-turn.component.scss'],
    imports: [
        CommonModule,
        TurnTimerComponent,
        MatExpansionModule,
        MatButtonModule,
        MatCardModule,
        MatIconModule,
        MatProgressBarModule,
        MatBadgeModule,
    ]
})
export class CurrentTurnComponent implements OnChanges {
  @Input()
  public game: Game | null = null;

  @Input()
  public players: Player[] | null = null;

  @Input()
  public currentPlayer: Player | null = null;

  @Output()
  public endTurn = new EventEmitter<void>();

  @Input()
  public isHost: boolean | null = false;

  public isMyTurn = false;

  public currentTurn: Player | undefined;

  public nextTurn: Player | undefined;

  // For turn progress indicators
  public turnStartTime = 0;
  public turnDuration = 0; // in milliseconds
  public turnProgress = 0; // 0-100 for progress bar

  // Default turn time in milliseconds (2 minutes)
  private readonly DEFAULT_TURN_TIME: number = 2 * 60 * 1000;

  constructor(private playerService: PlayerService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (
      (changes['game'] || changes['players']) &&
      !!this.game &&
      !!this.players &&
      !!this.currentPlayer
    ) {
      // Get the current turn player
      const currentTurnIdx = this.players?.findIndex(
        (p) => p.id === this.game?.currentTurnPlayerId,
      );

      if (currentTurnIdx >= 0) {
        this.currentTurn = this.players[currentTurnIdx];

        if (this.players.length > 1) {
          this.nextTurn =
            this.players[(currentTurnIdx + 1) % this.players.length];
        }
      }

      this.isMyTurn = this.currentTurn?.id === this.currentPlayer.id;

      // Initialize turn progress
      if (this.game.lastTurnStartTime) {
        this.turnStartTime = new Date(this.game.lastTurnStartTime).getTime();
        this.updateTurnProgress();

        // Update progress every second
        setInterval(() => this.updateTurnProgress(), 1000);
      }
    }
  }

  getPlayerColor(player?: Player): string {
    return this.playerService.getPlayerColor(player);
  }

  // Calculate turn progress as a percentage
  updateTurnProgress(): void {
    if (this.turnStartTime > 0) {
      const now = Date.now();
      this.turnDuration = now - this.turnStartTime;

      // Calculate progress as a percentage of DEFAULT_TURN_TIME
      this.turnProgress = Math.min(100, (this.turnDuration / this.DEFAULT_TURN_TIME) * 100);
    }
  }

  onEndTurn() {
    this.endTurn.emit();
  }
}
