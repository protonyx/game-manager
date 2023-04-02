import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FormControl, FormBuilder, Validators} from '@angular/forms';
import {JoinGame} from "../../models/models";

@Component({
    selector: 'app-join-game',
    templateUrl: './join-game.component.html',
    styleUrls: ['./join-game.component.scss']
})
export class JoinGameComponent {

    @Input()
    public loading: boolean | undefined;

    @Input()
    public errorMessage: string | undefined;

    @Output()
    public joinGame: EventEmitter<JoinGame> = new EventEmitter<JoinGame>();

    joinGameForm = this.fb.group({
        entryCode: ['', Validators.required],
        playerName: new FormControl('', Validators.required)
    });

    constructor(private fb: FormBuilder) {
    }

    onSubmit(): void {
        this.joinGame.emit({
            entryCode: this.joinGameForm.value.entryCode as string,
            name: this.joinGameForm.value.playerName as string
        });
    }
}
