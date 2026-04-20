import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  FormControl,
  FormBuilder,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { JoinGame } from '../../models/models';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PLAYER_COLORS } from '../../models/player-colors';

@Component({
    selector: 'app-join-game',
    templateUrl: './join-game.component.html',
    styleUrls: ['./join-game.component.scss'],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatProgressBarModule,
        MatIconModule,
        MatTooltipModule,
    ]
})
export class JoinGameComponent {
  @Input()
  public loading: boolean | undefined;

  @Input()
  public errorMessage: string | null = null;

  @Output()
  public joinGame: EventEmitter<JoinGame> = new EventEmitter<JoinGame>();

  readonly colors = PLAYER_COLORS;

  joinGameForm = this.fb.group({
    entryCode: ['', Validators.required],
    playerName: new FormControl('', Validators.required),
    color: new FormControl('', Validators.required),
  });

  constructor(private fb: FormBuilder) {}

  selectColor(hex: string): void {
    this.joinGameForm.patchValue({ color: hex });
  }

  isColorSelected(hex: string): boolean {
    return this.joinGameForm.value.color?.toLowerCase() === hex.toLowerCase();
  }

  onSubmit(): void {
    this.joinGame.emit({
      entryCode: this.joinGameForm.value.entryCode?.trim() as string,
      name: this.joinGameForm.value.playerName?.trim() as string,
      color: this.joinGameForm.value.color as string,
      observer: false,
    });
  }

  onJoinAsObserver(): void {
    this.joinGame.emit({
      entryCode: this.joinGameForm.value.entryCode?.trim() as string,
      name: this.joinGameForm.value.playerName?.trim() as string,
      observer: true,
    });
  }
}
