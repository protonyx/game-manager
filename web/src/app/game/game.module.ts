import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {JoinGameComponent} from "./components/join-game/join-game.component";
import {JoinGamePageComponent} from "./pages/join-game-page/join-game-page.component";
import {GamePageComponent} from "./pages/game-page/game-page.component";
import {CurrentTurnComponent} from "./components/current-turn/current-turn.component";
import {NewGameComponent} from "./components/new-game/new-game.component";
import {NewGamePageComponent} from "./pages/new-game-page/new-game-page.component";
import {GameRoutingModule} from "./game-routing.module";

import {PlayerListComponent} from './components/player-list/player-list.component';
import {TurnTimerComponent} from './components/turn-timer/turn-timer.component';
import { PlayerEditComponent } from './components/player-edit/player-edit.component';
import { TrackerEditorComponent } from './components/tracker-editor/tracker-editor.component';


@NgModule({
    imports: [
    CommonModule,
    GameRoutingModule,
    JoinGameComponent,
    JoinGamePageComponent,
    GamePageComponent,
    CurrentTurnComponent,
    NewGameComponent,
    NewGamePageComponent,
    PlayerListComponent,
    TurnTimerComponent,
    PlayerEditComponent,
    TrackerEditorComponent
]
})
export class GameModule {
}
