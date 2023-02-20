import { Component } from '@angular/core';
import {FormGroup, FormControl, FormBuilder, Validators} from '@angular/forms';

@Component({
  selector: 'app-join-game',
  templateUrl: './join-game.component.html',
  styleUrls: ['./join-game.component.scss']
})
export class JoinGameComponent {
  joinGameForm = this.fb.group({
    entryCode: ['', Validators.required],
    name: new FormControl('', Validators.required)
  });

  constructor(private fb: FormBuilder) {
  }

  onSubmit(): void {

  }
}
