import {Component, EventEmitter, Output} from '@angular/core';
import {FormControl, FormBuilder, Validators} from '@angular/forms';
import {JoinGame} from "../../models/models";

@Component({
    selector: 'app-join-game',
    templateUrl: './join-game.component.html',
    styleUrls: ['./join-game.component.scss']
})
export class JoinGameComponent {

    @Output()
    public joinGame: EventEmitter<JoinGame> = new EventEmitter<JoinGame>();

    joinGameForm = this.fb.group({
        entryCode: ['', Validators.required],
        name: new FormControl('', Validators.required)
    });

    constructor(private fb: FormBuilder) {
    }

    onSubmit(): void {
        this.joinGame.emit(this.joinGameForm.value as JoinGame);
    }
}
