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

@Component({
  selector: 'app-join-game',
  templateUrl: './join-game.component.html',
  styleUrls: ['./join-game.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressBarModule,
  ],
})
export class JoinGameComponent {
  @Input()
  public loading: boolean | undefined;

  @Input()
  public errorMessage: string | null = null;

  @Output()
  public joinGame: EventEmitter<JoinGame> = new EventEmitter<JoinGame>();

  joinGameForm = this.fb.group({
    entryCode: ['', Validators.required],
    playerName: new FormControl('', Validators.required),
  });

  constructor(private fb: FormBuilder) {}

  onSubmit(): void {
    this.joinGame.emit({
      entryCode: this.joinGameForm.value.entryCode?.trim() as string,
      name: this.joinGameForm.value.playerName?.trim() as string,
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
