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
import { TurnTimerComponent } from '../turn-timer/turn-timer.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-current-turn',
  templateUrl: './current-turn.component.html',
  styleUrls: ['./current-turn.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    TurnTimerComponent,
    MatExpansionModule,
    MatButtonModule,
  ],
})
export class CurrentTurnComponent implements OnChanges {
  @Input()
  public game: Game | null = null;

  @Input()
  public players: Player[] | null = null;

  @Input()
  public currentPlayer: Player | null = null;

  @Output()
  public endTurn: EventEmitter<void> = new EventEmitter();

  @Input()
  public isHost: boolean | null = false;

  public isMyTurn: boolean = false;

  public currentTurn: Player | undefined;

  public nextTurn: Player | undefined;

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
    }
  }

  onEndTurn() {
    this.endTurn.emit();
  }
}
